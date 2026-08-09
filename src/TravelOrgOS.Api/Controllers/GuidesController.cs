using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TravelOrgOS.Api.Authorization;
using TravelOrgOS.Api.DTOs;
using TravelOrgOS.Infrastructure.Services;

namespace TravelOrgOS.Api.Controllers;

[Route("api/[controller]")]
public class GuidesController : BaseApiController
{
    private readonly IGuideService _guideService;

    public GuidesController(IGuideService guideService)
    {
        _guideService = guideService;
    }

    [HttpGet]
    [RequiresPermission("Vendor.Manage")]
    public async Task<IActionResult> GetGuides([FromQuery] string? search)
    {
        var orgId = GetOrgId();
        var guides = await _guideService.GetGuidesAsync(orgId, search);
        return Ok(guides);
    }

    [HttpGet("{id}")]
    [RequiresPermission("Vendor.Manage")]
    public async Task<IActionResult> GetGuideById(Guid id)
    {
        var orgId = GetOrgId();
        var guide = await _guideService.GetGuideByIdAsync(orgId, id);
        if (guide == null) return NotFound("Guide not found.");
        return Ok(guide);
    }

    [HttpPost]
    [RequiresPermission("Vendor.Manage")]
    public async Task<IActionResult> CreateGuide([FromBody] CreateGuideDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var orgId = GetOrgId();
        try
        {
            var guide = await _guideService.CreateGuideAsync(orgId, dto);
            return CreatedAtAction(nameof(GetGuideById), new { id = guide.Id }, guide);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    [RequiresPermission("Vendor.Manage")]
    public async Task<IActionResult> UpdateGuide(Guid id, [FromBody] CreateGuideDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var orgId = GetOrgId();
        try
        {
            var guide = await _guideService.UpdateGuideAsync(orgId, id, dto);
            if (guide == null) return NotFound("Guide not found.");
            return Ok(guide);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/toggle-status")]
    [RequiresPermission("Vendor.Manage")]
    public async Task<IActionResult> ToggleStatus(Guid id)
    {
        var orgId = GetOrgId();
        var guide = await _guideService.ToggleStatusAsync(orgId, id);
        if (guide == null) return NotFound("Guide not found.");
        return Ok(guide);
    }

    [HttpGet("{id}/availability")]
    [RequiresPermission("Vendor.Manage")]
    public async Task<IActionResult> CheckAvailability(
        Guid id, 
        [FromQuery] DateTime start, 
        [FromQuery] DateTime end, 
        [FromQuery] Guid? currentTripId)
    {
        var orgId = GetOrgId();
        var isAvailable = await _guideService.CheckGuideAvailabilityAsync(orgId, id, start, end, currentTripId);
        return Ok(isAvailable);
    }
}
