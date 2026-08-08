using Microsoft.AspNetCore.Mvc;
using TravelOrgOS.Infrastructure.Services;

namespace TravelOrgOS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : BaseApiController
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboardSummary()
    {
        var result = await _dashboardService.GetDashboardSummaryAsync(GetOrgId());
        return Ok(result);
    }
}
