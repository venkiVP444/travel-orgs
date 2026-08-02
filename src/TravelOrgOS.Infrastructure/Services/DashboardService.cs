using Microsoft.EntityFrameworkCore;
using TravelOrgOS.Api.DTOs;
using TravelOrgOS.Domain.Enums;
using TravelOrgOS.Infrastructure.Data;

namespace TravelOrgOS.Infrastructure.Services;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetDashboardSummaryAsync(Guid orgId);
}

public class DashboardService : IDashboardService
{
    private readonly TravelOrgOSDbContext _context;

    public DashboardService(TravelOrgOSDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(Guid orgId)
    {
        var trips = await _context.Trips
            .Where(t => t.OrganizationId == orgId)
            .ToListAsync();

        var bookings = await _context.Bookings
            .Include(b => b.Trip)
            .Where(b => b.OrganizationId == orgId)
            .ToListAsync();

        var travellersCount = await _context.Travellers
            .Where(t => t.OrganizationId == orgId)
            .CountAsync();

        var totalTrips = trips.Count;
        var activeTrips = trips.Count(t => t.Status == TripStatus.RegistrationOpen || t.Status == TripStatus.AlmostFull || t.Status == TripStatus.InProgress);

        var totalBookings = bookings.Count;
        var confirmedBookings = bookings.Count(b => b.BookingStatus == BookingStatus.Confirmed);

        var totalRevenue = bookings.Sum(b => b.PaidAmount);
        var pendingPayments = bookings.Where(b => b.PaymentStatus == PaymentStatus.Pending).Sum(b => b.TotalAmount);
        var outstandingBalance = bookings.Sum(b => b.BalanceAmount);

        var upcomingTrips = trips
            .Where(t => t.StartDate >= DateTime.UtcNow)
            .OrderBy(t => t.StartDate)
            .Take(5)
            .Select(t => new UpcomingTripDto(
                t.Id,
                t.TripCode,
                t.TripName,
                t.Destination,
                t.StartDate,
                t.TotalCapacity,
                t.TotalCapacity - t.AvailableSeats,
                t.AvailableSeats,
                t.Status
            ))
            .ToList();

        var recentBookings = bookings
            .OrderByDescending(b => b.CreatedAt)
            .Take(5)
            .Select(b => new RecentBookingDto(
                b.Id,
                b.BookingReference,
                b.Trip?.TripName ?? "Unknown Trip",
                b.ContactEmail,
                b.NumberOfTravellers,
                b.TotalAmount,
                b.PaymentStatus,
                b.BookingStatus,
                b.BookingDate
            ))
            .ToList();

        // 6-Month Revenue and Booking Trends
        var trends = new List<BookingTrendDto>();
        for (int i = 5; i >= 0; i--)
        {
            var targetMonth = DateTime.UtcNow.AddMonths(-i);
            var monthName = targetMonth.ToString("MMM yyyy");

            var monthBookings = bookings
                .Where(b => b.BookingDate.Month == targetMonth.Month && b.BookingDate.Year == targetMonth.Year)
                .ToList();

            trends.Add(new BookingTrendDto(
                MonthLabel: monthName,
                BookingCount: monthBookings.Count,
                Revenue: monthBookings.Sum(b => b.PaidAmount)
            ));
        }

        return new DashboardSummaryDto(
            totalTrips,
            activeTrips,
            travellersCount,
            totalBookings,
            confirmedBookings,
            totalRevenue,
            pendingPayments,
            outstandingBalance,
            upcomingTrips,
            recentBookings,
            trends
        );
    }
}
