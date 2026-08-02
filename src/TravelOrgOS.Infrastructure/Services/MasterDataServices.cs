using Microsoft.EntityFrameworkCore;
using TravelOrgOS.Api.DTOs;
using TravelOrgOS.Domain.Entities;
using TravelOrgOS.Infrastructure.Data;

namespace TravelOrgOS.Infrastructure.Services;

public interface IMasterDataService
{
    Task<OrganizationDto?> GetOrganizationBySlugAsync(string slug);
    Task<OrganizationDto?> GetOrganizationByIdAsync(Guid orgId);
    Task<OrganizationDto?> UpdateOrganizationAsync(Guid orgId, UpdateOrganizationDto dto);
    Task<List<Hotel>> GetHotelsAsync(Guid orgId);
    Task<Hotel> CreateHotelAsync(Guid orgId, Hotel hotel);
    Task<List<Vehicle>> GetVehiclesAsync(Guid orgId);
    Task<Vehicle> CreateVehicleAsync(Guid orgId, Vehicle vehicle);
    Task<List<Vendor>> GetVendorsAsync(Guid orgId);
    Task<Vendor> CreateVendorAsync(Guid orgId, Vendor vendor);
    Task<List<Notification>> GetNotificationsAsync(Guid orgId);
    Task<bool> MarkNotificationReadAsync(Guid orgId, Guid notificationId);
}

public class MasterDataService : IMasterDataService
{
    private readonly TravelOrgOSDbContext _context;

    public MasterDataService(TravelOrgOSDbContext context)
    {
        _context = context;
    }

    public async Task<OrganizationDto?> GetOrganizationBySlugAsync(string slug)
    {
        var org = await _context.Organizations.FirstOrDefaultAsync(o => o.Slug.ToLower() == slug.ToLower());
        return org == null ? null : MapOrgToDto(org);
    }

    public async Task<OrganizationDto?> GetOrganizationByIdAsync(Guid orgId)
    {
        var org = await _context.Organizations.FirstOrDefaultAsync(o => o.Id == orgId);
        return org == null ? null : MapOrgToDto(org);
    }

    public async Task<OrganizationDto?> UpdateOrganizationAsync(Guid orgId, UpdateOrganizationDto dto)
    {
        var org = await _context.Organizations.FirstOrDefaultAsync(o => o.Id == orgId);
        if (org == null) return null;

        org.Name = dto.Name;
        org.LegalName = dto.LegalName;
        if (!string.IsNullOrWhiteSpace(dto.LogoUrl)) org.LogoUrl = dto.LogoUrl;
        org.PrimaryColor = dto.PrimaryColor;
        org.SecondaryColor = dto.SecondaryColor;
        org.WelcomeMessage = dto.WelcomeMessage;
        org.Email = dto.Email;
        org.Phone = dto.Phone;
        org.Website = dto.Website;
        org.Address = dto.Address;
        org.City = dto.City;
        org.Country = dto.Country;
        org.Description = dto.Description;
        org.WhatsAppNumber = dto.WhatsAppNumber;
        org.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return MapOrgToDto(org);
    }

    public async Task<List<Hotel>> GetHotelsAsync(Guid orgId) =>
        await _context.Hotels.Where(h => h.OrganizationId == orgId).OrderBy(h => h.HotelName).ToListAsync();

    public async Task<Hotel> CreateHotelAsync(Guid orgId, Hotel hotel)
    {
        hotel.Id = Guid.NewGuid();
        hotel.OrganizationId = orgId;
        hotel.CreatedAt = DateTime.UtcNow;
        _context.Hotels.Add(hotel);
        await _context.SaveChangesAsync();
        return hotel;
    }

    public async Task<List<Vehicle>> GetVehiclesAsync(Guid orgId) =>
        await _context.Vehicles.Where(v => v.OrganizationId == orgId).OrderBy(v => v.VehicleName).ToListAsync();

    public async Task<Vehicle> CreateVehicleAsync(Guid orgId, Vehicle vehicle)
    {
        vehicle.Id = Guid.NewGuid();
        vehicle.OrganizationId = orgId;
        vehicle.CreatedAt = DateTime.UtcNow;
        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync();
        return vehicle;
    }

    public async Task<List<Vendor>> GetVendorsAsync(Guid orgId) =>
        await _context.Vendors.Where(v => v.OrganizationId == orgId).OrderBy(v => v.VendorName).ToListAsync();

    public async Task<Vendor> CreateVendorAsync(Guid orgId, Vendor vendor)
    {
        vendor.Id = Guid.NewGuid();
        vendor.OrganizationId = orgId;
        vendor.CreatedAt = DateTime.UtcNow;
        _context.Vendors.Add(vendor);
        await _context.SaveChangesAsync();
        return vendor;
    }

    public async Task<List<Notification>> GetNotificationsAsync(Guid orgId) =>
        await _context.Notifications.Where(n => n.OrganizationId == orgId).OrderByDescending(n => n.CreatedAt).Take(20).ToListAsync();

    public async Task<bool> MarkNotificationReadAsync(Guid orgId, Guid notificationId)
    {
        var n = await _context.Notifications.FirstOrDefaultAsync(x => x.OrganizationId == orgId && x.Id == notificationId);
        if (n == null) return false;
        n.IsRead = true;
        await _context.SaveChangesAsync();
        return true;
    }

    private static OrganizationDto MapOrgToDto(Organization o) => new(
        o.Id, o.Name, o.Slug, o.LegalName, o.LogoUrl, o.PrimaryColor, o.SecondaryColor,
        o.WelcomeMessage, o.Email, o.Phone, o.Website, o.Address, o.City, o.Country,
        o.BusinessHours, o.Description, o.FacebookUrl, o.InstagramUrl, o.LinkedInUrl, o.WhatsAppNumber, o.Status
    );
}
