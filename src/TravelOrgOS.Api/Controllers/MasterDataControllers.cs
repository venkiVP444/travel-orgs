using Microsoft.AspNetCore.Mvc;
using TravelOrgOS.Api.DTOs;
using TravelOrgOS.Domain.Entities;
using TravelOrgOS.Infrastructure.Services;

namespace TravelOrgOS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrganizationsController : ControllerBase
{
    private readonly IMasterDataService _service;

    public OrganizationsController(IMasterDataService service)
    {
        _service = service;
    }

    private Guid GetOrgId()
    {
        var claim = User.FindFirst("OrganizationId")?.Value;
        if (Guid.TryParse(claim, out var orgId)) return orgId;
        return Guid.Parse("11111111-1111-1111-1111-111111111111");
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentOrg()
    {
        var org = await _service.GetOrganizationByIdAsync(GetOrgId());
        if (org == null) return NotFound();
        return Ok(org);
    }

    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetOrgBySlug(string slug)
    {
        var org = await _service.GetOrganizationBySlugAsync(slug);
        if (org == null) return NotFound();
        return Ok(org);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateOrg([FromBody] UpdateOrganizationDto dto)
    {
        var result = await _service.UpdateOrganizationAsync(GetOrgId(), dto);
        if (result == null) return NotFound();
        return Ok(result);
    }
}

[ApiController]
[Route("api/[controller]")]
public class HotelsController : ControllerBase
{
    private readonly IMasterDataService _service;

    public HotelsController(IMasterDataService service) => _service = service;

    private Guid GetOrgId() => Guid.Parse("11111111-1111-1111-1111-111111111111");

    [HttpGet]
    public async Task<IActionResult> GetHotels() => Ok(await _service.GetHotelsAsync(GetOrgId()));

    [HttpPost]
    public async Task<IActionResult> CreateHotel([FromBody] Hotel hotel) => Ok(await _service.CreateHotelAsync(GetOrgId(), hotel));
}

[ApiController]
[Route("api/[controller]")]
public class VehiclesController : ControllerBase
{
    private readonly IMasterDataService _service;

    public VehiclesController(IMasterDataService service) => _service = service;

    private Guid GetOrgId() => Guid.Parse("11111111-1111-1111-1111-111111111111");

    [HttpGet]
    public async Task<IActionResult> GetVehicles() => Ok(await _service.GetVehiclesAsync(GetOrgId()));

    [HttpPost]
    public async Task<IActionResult> CreateVehicle([FromBody] Vehicle vehicle) => Ok(await _service.CreateVehicleAsync(GetOrgId(), vehicle));
}

[ApiController]
[Route("api/[controller]")]
public class VendorsController : ControllerBase
{
    private readonly IMasterDataService _service;

    public VendorsController(IMasterDataService service) => _service = service;

    private Guid GetOrgId() => Guid.Parse("11111111-1111-1111-1111-111111111111");

    [HttpGet]
    public async Task<IActionResult> GetVendors() => Ok(await _service.GetVendorsAsync(GetOrgId()));

    [HttpPost]
    public async Task<IActionResult> CreateVendor([FromBody] Vendor vendor) => Ok(await _service.CreateVendorAsync(GetOrgId(), vendor));
}

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly IMasterDataService _service;

    public NotificationsController(IMasterDataService service) => _service = service;

    private Guid GetOrgId() => Guid.Parse("11111111-1111-1111-1111-111111111111");

    [HttpGet]
    public async Task<IActionResult> GetNotifications() => Ok(await _service.GetNotificationsAsync(GetOrgId()));

    [HttpPost("{id:guid}/read")]
    public async Task<IActionResult> MarkRead(Guid id) => Ok(await _service.MarkNotificationReadAsync(GetOrgId(), id));
}
