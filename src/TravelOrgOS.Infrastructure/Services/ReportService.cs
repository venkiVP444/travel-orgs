using System.Text;
using Microsoft.EntityFrameworkCore;
using TravelOrgOS.Infrastructure.Data;

namespace TravelOrgOS.Infrastructure.Services;

public interface IReportService
{
    Task<byte[]> ExportBookingsCsvAsync(Guid orgId);
    Task<byte[]> ExportTravellersCsvAsync(Guid orgId);
    Task<byte[]> ExportRevenueCsvAsync(Guid orgId);
    Task<byte[]> ExportOutstandingPaymentsCsvAsync(Guid orgId);
}

public class ReportService : IReportService
{
    private readonly TravelOrgOSDbContext _context;

    public ReportService(TravelOrgOSDbContext context)
    {
        _context = context;
    }

    public async Task<byte[]> ExportBookingsCsvAsync(Guid orgId)
    {
        var bookings = await _context.Bookings
            .Include(b => b.Trip)
            .Where(b => b.OrganizationId == orgId)
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync();

        var sb = new StringBuilder();
        sb.AppendLine("BookingReference,TripCode,TripName,BookingDate,ContactEmail,ContactPhone,Passengers,TotalAmount,PaidAmount,BalanceAmount,PaymentStatus,BookingStatus");

        foreach (var b in bookings)
        {
            sb.AppendLine($"\"{b.BookingReference}\",\"{b.Trip?.TripCode}\",\"{b.Trip?.TripName}\",\"{b.BookingDate:yyyy-MM-dd}\",\"{b.ContactEmail}\",\"{b.ContactPhone}\",{b.NumberOfTravellers},{b.TotalAmount},{b.PaidAmount},{b.BalanceAmount},\"{b.PaymentStatus}\",\"{b.BookingStatus}\"");
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    public async Task<byte[]> ExportTravellersCsvAsync(Guid orgId)
    {
        var travellers = await _context.Travellers
            .Where(t => t.OrganizationId == orgId)
            .OrderBy(t => t.LastName)
            .ToListAsync();

        var sb = new StringBuilder();
        sb.AppendLine("FirstName,LastName,Email,MobileNumber,DateOfBirth,Gender,Nationality,PassportNumber,City,Country,CreatedAt");

        foreach (var t in travellers)
        {
            sb.AppendLine($"\"{t.FirstName}\",\"{t.LastName}\",\"{t.Email}\",\"{t.MobileNumber}\",\"{t.DateOfBirth:yyyy-MM-dd}\",\"{t.Gender}\",\"{t.Nationality}\",\"{t.PassportNumber}\",\"{t.City}\",\"{t.Country}\",\"{t.CreatedAt:yyyy-MM-dd}\"");
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    public async Task<byte[]> ExportRevenueCsvAsync(Guid orgId)
    {
        var payments = await _context.Payments
            .Include(p => p.Booking).ThenInclude(b => b!.Trip)
            .Where(p => p.OrganizationId == orgId)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();

        var sb = new StringBuilder();
        sb.AppendLine("TransactionReference,PaymentDate,BookingReference,TripName,PaymentMethod,Amount,Status");

        foreach (var p in payments)
        {
            sb.AppendLine($"\"{p.TransactionReference}\",\"{p.PaymentDate:yyyy-MM-dd HH:mm}\",\"{p.Booking?.BookingReference}\",\"{p.Booking?.Trip?.TripName}\",\"{p.PaymentMethod}\",{p.Amount},\"{p.Status}\"");
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    public async Task<byte[]> ExportOutstandingPaymentsCsvAsync(Guid orgId)
    {
        var pendingBookings = await _context.Bookings
            .Include(b => b.Trip)
            .Where(b => b.OrganizationId == orgId && b.BalanceAmount > 0)
            .OrderByDescending(b => b.BalanceAmount)
            .ToListAsync();

        var sb = new StringBuilder();
        sb.AppendLine("BookingReference,TripCode,TripName,ContactEmail,ContactPhone,TotalAmount,PaidAmount,OutstandingBalance,PaymentStatus");

        foreach (var b in pendingBookings)
        {
            sb.AppendLine($"\"{b.BookingReference}\",\"{b.Trip?.TripCode}\",\"{b.Trip?.TripName}\",\"{b.ContactEmail}\",\"{b.ContactPhone}\",{b.TotalAmount},{b.PaidAmount},{b.BalanceAmount},\"{b.PaymentStatus}\"");
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }
}
