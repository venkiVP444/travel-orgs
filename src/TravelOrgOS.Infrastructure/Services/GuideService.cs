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

public interface IGuideService
{
    Task<List<GuideDto>> GetGuidesAsync(Guid orgId, string? search = null);
    Task<GuideDto?> GetGuideByIdAsync(Guid orgId, Guid id);
    Task<GuideDto> CreateGuideAsync(Guid orgId, CreateGuideDto dto);
    Task<GuideDto?> UpdateGuideAsync(Guid orgId, Guid id, CreateGuideDto dto);
    Task<GuideDto?> ToggleStatusAsync(Guid orgId, Guid id);
    Task<bool> CheckGuideAvailabilityAsync(Guid orgId, Guid guideId, DateTime startDate, DateTime endDate, Guid? currentTripId = null);
}

public class GuideService : IGuideService
{
    private readonly TravelOrgOSDbContext _context;

    public GuideService(TravelOrgOSDbContext context)
    {
        _context = context;
    }

    public async Task<List<GuideDto>> GetGuidesAsync(Guid orgId, string? search = null)
    {
        var query = _context.Guides.Where(g => g.OrganizationId == orgId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.ToLower();
            query = query.Where(g => g.Name.ToLower().Contains(s) || g.Languages.ToLower().Contains(s) || g.Specializations.ToLower().Contains(s));
        }

        var guides = await query.OrderBy(g => g.Name).ToListAsync();
        return guides.Select(MapToDto).ToList();
    }

    public async Task<GuideDto?> GetGuideByIdAsync(Guid orgId, Guid id)
    {
        var guide = await _context.Guides.FirstOrDefaultAsync(g => g.OrganizationId == orgId && g.Id == id);
        return guide == null ? null : MapToDto(guide);
    }

    public async Task<GuideDto> CreateGuideAsync(Guid orgId, CreateGuideDto dto)
    {
        // Enforce uniqueness of email within organization scope
        var emailExists = await _context.Guides.AnyAsync(g => g.OrganizationId == orgId && g.Email.ToLower() == dto.Email.ToLower());
        if (emailExists)
        {
            throw new InvalidOperationException("A guide with this email is already registered.");
        }

        var guide = new Guide
        {
            Id = Guid.NewGuid(),
            OrganizationId = orgId,
            Name = dto.Name,
            Phone = dto.Phone,
            Email = dto.Email,
            Languages = dto.Languages,
            Specializations = dto.Specializations,
            ExperienceYears = dto.ExperienceYears,
            LicenseNumber = dto.LicenseNumber,
            Status = true,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow
        };

        _context.Guides.Add(guide);
        await _context.SaveChangesAsync();
        return MapToDto(guide);
    }

    public async Task<GuideDto?> UpdateGuideAsync(Guid orgId, Guid id, CreateGuideDto dto)
    {
        var guide = await _context.Guides.FirstOrDefaultAsync(g => g.OrganizationId == orgId && g.Id == id);
        if (guide == null) return null;

        var emailExists = await _context.Guides.AnyAsync(g => g.OrganizationId == orgId && g.Id != id && g.Email.ToLower() == dto.Email.ToLower());
        if (emailExists)
        {
            throw new InvalidOperationException("A guide with this email is already registered.");
        }

        guide.Name = dto.Name;
        guide.Phone = dto.Phone;
        guide.Email = dto.Email;
        guide.Languages = dto.Languages;
        guide.Specializations = dto.Specializations;
        guide.ExperienceYears = dto.ExperienceYears;
        guide.LicenseNumber = dto.LicenseNumber;
        guide.Notes = dto.Notes;
        guide.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return MapToDto(guide);
    }

    public async Task<GuideDto?> ToggleStatusAsync(Guid orgId, Guid id)
    {
        var guide = await _context.Guides.FirstOrDefaultAsync(g => g.OrganizationId == orgId && g.Id == id);
        if (guide == null) return null;

        guide.Status = !guide.Status;
        guide.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return MapToDto(guide);
    }

    public async Task<bool> CheckGuideAvailabilityAsync(Guid orgId, Guid guideId, DateTime startDate, DateTime endDate, Guid? currentTripId = null)
    {
        // Resolve target guide status
        var guide = await _context.Guides.FirstOrDefaultAsync(g => g.OrganizationId == orgId && g.Id == guideId);
        if (guide == null || !guide.Status)
        {
            return false; // Guide must exist and be active
        }

        // Verify overlapping assignments in active/upcoming trips
        var overlap = await _context.TripGuides
            .Include(tg => tg.Trip)
            .AnyAsync(tg =>
                tg.GuideId == guideId &&
                tg.Trip!.OrganizationId == orgId &&
                tg.TripId != currentTripId &&
                tg.Trip.Status != TripStatus.Cancelled &&
                tg.Trip.StartDate <= endDate &&
                tg.Trip.EndDate >= startDate);

        return !overlap;
    }

    private static GuideDto MapToDto(Guide g) => new(
        g.Id,
        g.OrganizationId,
        g.Name,
        g.Phone,
        g.Email,
        g.Languages,
        g.Specializations,
        g.ExperienceYears,
        g.LicenseNumber,
        g.Status,
        g.Notes,
        g.CreatedAt
    );
}
