using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TravelOrgOS.Api.Authorization;
using TravelOrgOS.Api.DTOs;
using TravelOrgOS.Domain.Enums;
using TravelOrgOS.Infrastructure.Services;

namespace TravelOrgOS.Api.Controllers;

[Route("api/[controller]")]
public class SubscriptionsController : BaseApiController
{
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionsController(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    [HttpGet("quota")]
    [RequiresPermission("Subscription.View")]
    public async Task<IActionResult> GetQuota()
    {
        var orgId = GetOrgId();
        var quota = await _subscriptionService.GetQuotaAsync(orgId);
        if (quota == null)
        {
            // Auto initialize Starter tier if missing
            var starter = await _subscriptionService.InitializeQuotaAsync(orgId, SubscriptionTier.Starter);
            return Ok(starter);
        }
        return Ok(quota);
    }

    [HttpPost("initialize")]
    [RequiresPermission("Subscription.Manage")] // PlatformAdmin or special roles
    public async Task<IActionResult> InitializeQuota([FromBody] SubscriptionTier tier)
    {
        var orgId = GetOrgId();
        var quota = await _subscriptionService.InitializeQuotaAsync(orgId, tier);
        return Ok(quota);
    }
}
