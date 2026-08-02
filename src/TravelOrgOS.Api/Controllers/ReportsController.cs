using Microsoft.AspNetCore.Mvc;
using TravelOrgOS.Infrastructure.Services;

namespace TravelOrgOS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    private Guid GetOrgId()
    {
        var claim = User.FindFirst("OrganizationId")?.Value;
        if (Guid.TryParse(claim, out var orgId)) return orgId;
        return Guid.Parse("11111111-1111-1111-1111-111111111111");
    }

    [HttpGet("bookings/export")]
    public async Task<IActionResult> ExportBookings()
    {
        var bytes = await _reportService.ExportBookingsCsvAsync(GetOrgId());
        return File(bytes, "text/csv", $"Bookings_Report_{DateTime.UtcNow:yyyyMMdd}.csv");
    }

    [HttpGet("travellers/export")]
    public async Task<IActionResult> ExportTravellers()
    {
        var bytes = await _reportService.ExportTravellersCsvAsync(GetOrgId());
        return File(bytes, "text/csv", $"Travellers_Report_{DateTime.UtcNow:yyyyMMdd}.csv");
    }

    [HttpGet("revenue/export")]
    public async Task<IActionResult> ExportRevenue()
    {
        var bytes = await _reportService.ExportRevenueCsvAsync(GetOrgId());
        return File(bytes, "text/csv", $"Revenue_Report_{DateTime.UtcNow:yyyyMMdd}.csv");
    }

    [HttpGet("outstanding/export")]
    public async Task<IActionResult> ExportOutstanding()
    {
        var bytes = await _reportService.ExportOutstandingPaymentsCsvAsync(GetOrgId());
        return File(bytes, "text/csv", $"Outstanding_Payments_Report_{DateTime.UtcNow:yyyyMMdd}.csv");
    }
}
