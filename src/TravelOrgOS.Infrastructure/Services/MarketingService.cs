using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TravelOrgOS.Api.DTOs;
using TravelOrgOS.Domain.Entities;
using TravelOrgOS.Domain.Enums;
using TravelOrgOS.Infrastructure.Data;

namespace TravelOrgOS.Infrastructure.Services;

public interface IMarketingService
{
    Task<List<CampaignDto>> GetCampaignsAsync(Guid orgId);
    Task<CampaignDto> CreateCampaignAsync(Guid orgId, CreateCampaignDto dto);
    Task<bool> SendCampaignNowAsync(Guid orgId, Guid campaignId);
}

public class MarketingService : IMarketingService
{
    private readonly TravelOrgOSDbContext _context;

    public MarketingService(TravelOrgOSDbContext context)
    {
        _context = context;
    }

    public async Task<List<CampaignDto>> GetCampaignsAsync(Guid orgId)
    {
        var campaigns = await _context.Campaigns
            .Where(c => c.OrganizationId == orgId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return campaigns.Select(MapToDto).ToList();
    }

    public async Task<CampaignDto> CreateCampaignAsync(Guid orgId, CreateCampaignDto dto)
    {
        var campaign = new Campaign
        {
            Id = Guid.NewGuid(),
            OrganizationId = orgId,
            Name = dto.Name,
            Type = dto.Type,
            Status = CampaignStatus.Draft,
            Subject = dto.Subject,
            BodyTemplate = dto.BodyTemplate,
            TargetSegmentQuery = dto.TargetSegmentQuery,
            ScheduledFor = dto.ScheduledFor,
            CreatedAt = DateTime.UtcNow
        };

        _context.Campaigns.Add(campaign);
        await _context.SaveChangesAsync();
        return MapToDto(campaign);
    }

    public async Task<bool> SendCampaignNowAsync(Guid orgId, Guid campaignId)
    {
        var campaign = await _context.Campaigns
            .Include(c => c.Recipients)
            .FirstOrDefaultAsync(c => c.OrganizationId == orgId && c.Id == campaignId);

        if (campaign == null || campaign.Status == CampaignStatus.Sent)
        {
            return false;
        }

        campaign.Status = CampaignStatus.Sending;
        await _context.SaveChangesAsync();

        // 1. Resolve target segment recipients
        IQueryable<Traveller> travellerQuery = _context.Travellers.Where(t => t.OrganizationId == orgId && t.Status);

        if (string.Equals(campaign.TargetSegmentQuery, "past-travellers", StringComparison.OrdinalIgnoreCase))
        {
            travellerQuery = travellerQuery.Where(t => t.Bookings.Any(b => b.Booking!.BookingStatus == BookingStatus.Completed || b.Booking.BookingStatus == BookingStatus.Confirmed));
        }
        else if (string.Equals(campaign.TargetSegmentQuery, "inactive-customers", StringComparison.OrdinalIgnoreCase))
        {
            // Travellers with no bookings
            travellerQuery = travellerQuery.Where(t => !t.Bookings.Any());
        }

        var targets = await travellerQuery.ToListAsync();

        // 2. Dispatch simulated campaign events
        foreach (var t in targets)
        {
            var recipient = new CampaignRecipient
            {
                Id = Guid.NewGuid(),
                CampaignId = campaign.Id,
                TravellerId = t.Id,
                Status = "Sent",
                ActionAt = DateTime.UtcNow
            };
            _context.CampaignRecipients.Add(recipient);
        }

        campaign.Status = CampaignStatus.Sent;
        campaign.SentAt = DateTime.UtcNow;
        campaign.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    private static CampaignDto MapToDto(Campaign c) => new(
        c.Id,
        c.OrganizationId,
        c.Name,
        c.Type,
        c.Status,
        c.Subject,
        c.BodyTemplate,
        c.TargetSegmentQuery,
        c.ScheduledFor,
        c.SentAt,
        c.CreatedAt
    );
}
