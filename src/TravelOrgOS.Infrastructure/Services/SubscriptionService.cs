using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TravelOrgOS.Api.DTOs;
using TravelOrgOS.Domain.Entities;
using TravelOrgOS.Domain.Enums;
using TravelOrgOS.Infrastructure.Data;

namespace TravelOrgOS.Infrastructure.Services;

public interface ISubscriptionService
{
    Task<SubscriptionQuotaDto?> GetQuotaAsync(Guid orgId);
    Task<SubscriptionQuotaDto> InitializeQuotaAsync(Guid orgId, SubscriptionTier tier);
    Task<bool> CanCreateTripAsync(Guid orgId);
    Task<bool> CanCreateBookingAsync(Guid orgId);
    Task<bool> CanInviteMemberAsync(Guid orgId);
}

public class SubscriptionService : ISubscriptionService
{
    private readonly TravelOrgOSDbContext _context;

    public SubscriptionService(TravelOrgOSDbContext context)
    {
        _context = context;
    }

    public async Task<SubscriptionQuotaDto?> GetQuotaAsync(Guid orgId)
    {
        var quota = await _context.SubscriptionQuotas.FirstOrDefaultAsync(q => q.OrganizationId == orgId);
        return quota == null ? null : MapToDto(quota);
    }

    public async Task<SubscriptionQuotaDto> InitializeQuotaAsync(Guid orgId, SubscriptionTier tier)
    {
        var existing = await _context.SubscriptionQuotas.FirstOrDefaultAsync(q => q.OrganizationId == orgId);
        if (existing != null)
        {
            existing.Tier = tier;
            ConfigureLimits(existing, tier);
            existing.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return MapToDto(existing);
        }

        var quota = new SubscriptionQuota
        {
            Id = Guid.NewGuid(),
            OrganizationId = orgId,
            Tier = tier,
            Status = true,
            ExpiryDate = DateTime.UtcNow.AddDays(30),
            CreatedAt = DateTime.UtcNow
        };
        ConfigureLimits(quota, tier);

        _context.SubscriptionQuotas.Add(quota);
        await _context.SaveChangesAsync();
        return MapToDto(quota);
    }

    private static void ConfigureLimits(SubscriptionQuota q, SubscriptionTier tier)
    {
        switch (tier)
        {
            case SubscriptionTier.Starter:
                q.MaxActiveTrips = 3;
                q.MaxTeamMembers = 2;
                q.MaxBookingsPerMonth = 10;
                break;
            case SubscriptionTier.Growth:
                q.MaxActiveTrips = 10;
                q.MaxTeamMembers = 5;
                q.MaxBookingsPerMonth = 50;
                break;
            case SubscriptionTier.Business:
                q.MaxActiveTrips = 50;
                q.MaxTeamMembers = 15;
                q.MaxBookingsPerMonth = 250;
                break;
            case SubscriptionTier.Enterprise:
            default:
                q.MaxActiveTrips = 9999;
                q.MaxTeamMembers = 9999;
                q.MaxBookingsPerMonth = 9999;
                break;
        }
    }

    public async Task<bool> CanCreateTripAsync(Guid orgId)
    {
        var quota = await _context.SubscriptionQuotas.FirstOrDefaultAsync(q => q.OrganizationId == orgId);
        if (quota == null) return true; // Default fallback to bypass check

        var count = await _context.Trips.CountAsync(t => t.OrganizationId == orgId && t.Status != TripStatus.Cancelled);
        return count < quota.MaxActiveTrips;
    }

    public async Task<bool> CanCreateBookingAsync(Guid orgId)
    {
        var quota = await _context.SubscriptionQuotas.FirstOrDefaultAsync(q => q.OrganizationId == orgId);
        if (quota == null) return true;

        var currentMonthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        var count = await _context.Bookings.CountAsync(b => b.OrganizationId == orgId && b.BookingDate >= currentMonthStart);
        return count < quota.MaxBookingsPerMonth;
    }

    public async Task<bool> CanInviteMemberAsync(Guid orgId)
    {
        var quota = await _context.SubscriptionQuotas.FirstOrDefaultAsync(q => q.OrganizationId == orgId);
        if (quota == null) return true;

        var count = await _context.OrganizationUsers.CountAsync(u => u.OrganizationId == orgId);
        return count < quota.MaxTeamMembers;
    }

    private static SubscriptionQuotaDto MapToDto(SubscriptionQuota q) => new(
        q.Id,
        q.OrganizationId,
        q.Tier,
        q.MaxActiveTrips,
        q.MaxTeamMembers,
        q.MaxBookingsPerMonth,
        q.Status,
        q.ExpiryDate,
        q.CreatedAt
    );
}
