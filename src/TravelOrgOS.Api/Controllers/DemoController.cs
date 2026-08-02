using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelOrgOS.Infrastructure.Data;

namespace TravelOrgOS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DemoController : ControllerBase
{
    private readonly TravelOrgOSDbContext _context;

    public DemoController(TravelOrgOSDbContext context)
    {
        _context = context;
    }

    [HttpPost("reset")]
    public async Task<IActionResult> ResetDemoData()
    {
        // Safety verification
        DatabaseSafetyChecker.AssertConnectionIsLocalDbOnly(_context.Database.GetConnectionString() ?? "");

        _context.Payments.RemoveRange(_context.Payments);
        _context.BookingTravellers.RemoveRange(_context.BookingTravellers);
        _context.Bookings.RemoveRange(_context.Bookings);
        _context.TripMeals.RemoveRange(_context.TripMeals);
        _context.TripVendors.RemoveRange(_context.TripVendors);
        _context.TripVehicles.RemoveRange(_context.TripVehicles);
        _context.TripHotels.RemoveRange(_context.TripHotels);
        _context.TripItineraryDays.RemoveRange(_context.TripItineraryDays);
        _context.Trips.RemoveRange(_context.Trips);
        _context.Vendors.RemoveRange(_context.Vendors);
        _context.Vehicles.RemoveRange(_context.Vehicles);
        _context.Hotels.RemoveRange(_context.Hotels);
        _context.Travellers.RemoveRange(_context.Travellers);
        _context.OrganizationUsers.RemoveRange(_context.OrganizationUsers);
        _context.Organizations.RemoveRange(_context.Organizations);
        _context.Notifications.RemoveRange(_context.Notifications);
        _context.AuditLogs.RemoveRange(_context.AuditLogs);

        await _context.SaveChangesAsync();

        await DbInitializer.SeedAsync(_context);

        return Ok(new { message = "Demo data successfully reset to clean state!" });
    }
}
