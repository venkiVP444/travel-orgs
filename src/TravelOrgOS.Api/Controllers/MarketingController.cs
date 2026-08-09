using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TravelOrgOS.Api.Authorization;
using TravelOrgOS.Api.DTOs;
using TravelOrgOS.Infrastructure.Services;

namespace TravelOrgOS.Api.Controllers;

[Route("api/[controller]")]
public class MarketingController : BaseApiController
{
    private readonly IMarketingService _marketingService;

    public MarketingController(IMarketingService marketingService)
    {
        _marketingService = marketingService;
    }

    [HttpGet]
    [RequiresPermission("Marketing.Manage")]
    public async Task<IActionResult> GetCampaigns()
    {
        var orgId = GetOrgId();
        var campaigns = await _marketingService.GetCampaignsAsync(orgId);
        return Ok(campaigns);
    }

    [HttpPost]
    [RequiresPermission("Marketing.Manage")]
    public async Task<IActionResult> CreateCampaign([FromBody] CreateCampaignDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var orgId = GetOrgId();
        var campaign = await _marketingService.CreateCampaignAsync(orgId, dto);
        return Ok(campaign);
    }

    [HttpPost("{id}/send")]
    [RequiresPermission("Marketing.Manage")]
    public async Task<IActionResult> SendCampaign(Guid id)
    {
        var orgId = GetOrgId();
        var success = await _marketingService.SendCampaignNowAsync(orgId, id);
        if (!success) return BadRequest("Campaign could not be sent or is already processed.");
        return Ok(new { Success = true, Message = "Campaign dispatched successfully." });
    }
}
