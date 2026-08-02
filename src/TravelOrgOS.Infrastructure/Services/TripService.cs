using Microsoft.EntityFrameworkCore;
using TravelOrgOS.Api.DTOs;
using TravelOrgOS.Domain.Entities;
using TravelOrgOS.Domain.Enums;
using TravelOrgOS.Infrastructure.Data;

namespace TravelOrgOS.Infrastructure.Services;

public interface ITripService
{
    Task<List<TripDto>> GetTripsAsync(Guid orgId, string? search = null, TripStatus? status = null, bool publicOnly = false);
    Task<TripDto?> GetTripByIdAsync(Guid orgId, Guid id, bool publicOnly = false);
    Task<TripDto?> GetTripBySlugOrIdForPortalAsync(string orgSlug, Guid tripId);
    Task<TripDto> CreateTripAsync(Guid orgId, CreateTripDto dto);
    Task<TripDto?> UpdateTripAsync(Guid orgId, Guid id, CreateTripDto dto);
    Task<bool> PublishTripAsync(Guid orgId, Guid id);
    Task<bool> UnpublishTripAsync(Guid orgId, Guid id);
    Task<TripDto?> DuplicateTripAsync(Guid orgId, Guid id);
    Task<bool> DeleteTripAsync(Guid orgId, Guid id);

    Task<List<ItineraryDayDto>> SaveItineraryDaysAsync(Guid orgId, Guid tripId, List<ItineraryDayDto> days);
    Task<List<TripHotelDto>> SaveTripHotelsAsync(Guid orgId, Guid tripId, List<TripHotelDto> hotels);
    Task<List<TripVehicleDto>> SaveTripVehiclesAsync(Guid orgId, Guid tripId, List<TripVehicleDto> vehicles);
    Task<List<TripVendorDto>> SaveTripVendorsAsync(Guid orgId, Guid tripId, List<TripVendorDto> vendors);
    Task<List<TripMealDto>> SaveTripMealsAsync(Guid orgId, Guid tripId, List<TripMealDto> meals);
}

public class TripService : ITripService
{
    private readonly TravelOrgOSDbContext _context;

    public TripService(TravelOrgOSDbContext context)
    {
        _context = context;
    }

    public async Task<List<TripDto>> GetTripsAsync(Guid orgId, string? search = null, TripStatus? status = null, bool publicOnly = false)
    {
        var query = _context.Trips
            .Include(t => t.ItineraryDays)
            .Include(t => t.TripHotels).ThenInclude(th => th.Hotel)
            .Include(t => t.TripVehicles).ThenInclude(tv => tv.Vehicle)
            .Include(t => t.TripVendors).ThenInclude(tv => tv.Vendor)
            .Include(t => t.TripMeals)
            .Where(t => t.OrganizationId == orgId);

        if (publicOnly)
        {
            query = query.Where(t => t.Visibility == TripVisibility.Public && t.Status != TripStatus.Draft && t.Status != TripStatus.Cancelled);
        }

        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.ToLower();
            query = query.Where(t =>
                t.TripName.ToLower().Contains(s) ||
                t.Destination.ToLower().Contains(s) ||
                t.TripCode.ToLower().Contains(s));
        }

        var trips = await query.OrderByDescending(t => t.CreatedAt).ToListAsync();
        return trips.Select(t => MapToDto(t)).ToList();
    }

    public async Task<TripDto?> GetTripByIdAsync(Guid orgId, Guid id, bool publicOnly = false)
    {
        var query = _context.Trips
            .Include(t => t.ItineraryDays)
            .Include(t => t.TripHotels).ThenInclude(th => th.Hotel)
            .Include(t => t.TripVehicles).ThenInclude(tv => tv.Vehicle)
            .Include(t => t.TripVendors).ThenInclude(tv => tv.Vendor)
            .Include(t => t.TripMeals)
            .Where(t => t.OrganizationId == orgId && t.Id == id);

        if (publicOnly)
        {
            query = query.Where(t => t.Visibility == TripVisibility.Public && t.Status != TripStatus.Draft);
        }

        var trip = await query.FirstOrDefaultAsync();
        return trip == null ? null : MapToDto(trip);
    }

    public async Task<TripDto?> GetTripBySlugOrIdForPortalAsync(string orgSlug, Guid tripId)
    {
        var org = await _context.Organizations.FirstOrDefaultAsync(o => o.Slug.ToLower() == orgSlug.ToLower());
        if (org == null) return null;

        return await GetTripByIdAsync(org.Id, tripId, publicOnly: true);
    }

    public async Task<TripDto> CreateTripAsync(Guid orgId, CreateTripDto dto)
    {
        var trip = new Trip
        {
            Id = Guid.NewGuid(),
            OrganizationId = orgId,
            TripCode = dto.TripCode.Trim().ToUpper(),
            TripName = dto.TripName.Trim(),
            ShortDescription = dto.ShortDescription ?? string.Empty,
            Description = dto.Description ?? string.Empty,
            Destination = dto.Destination.Trim(),
            StartLocation = dto.StartLocation ?? string.Empty,
            EndLocation = dto.EndLocation ?? string.Empty,
            ViaRoute = dto.ViaRoute,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            DurationDays = dto.DurationDays > 0 ? dto.DurationDays : (int)(dto.EndDate - dto.StartDate).TotalDays + 1,
            DurationNights = dto.DurationNights >= 0 ? dto.DurationNights : (int)(dto.EndDate - dto.StartDate).TotalDays,
            TripType = dto.TripType,
            Status = TripStatus.Draft,
            Visibility = dto.Visibility,
            CoverImageUrl = !string.IsNullOrWhiteSpace(dto.CoverImageUrl) ? dto.CoverImageUrl : "https://images.unsplash.com/photo-1488646953014-85cb44e25828?w=800&fit=crop",
            BasePrice = dto.BasePrice,
            Currency = dto.Currency ?? "USD",
            TotalCapacity = dto.TotalCapacity,
            AvailableSeats = dto.TotalCapacity,
            MinimumTravellers = dto.MinimumTravellers,
            MaximumTravellers = dto.MaximumTravellers > 0 ? dto.MaximumTravellers : dto.TotalCapacity,
            HostGuide = dto.HostGuide,
            DriverName = dto.DriverName,
            EstimatedCost = dto.EstimatedCost,
            ContactPerson = dto.ContactPerson,
            ContactNumber = dto.ContactNumber,
            CreatedAt = DateTime.UtcNow
        };

        _context.Trips.Add(trip);
        await _context.SaveChangesAsync();

        return MapToDto(trip);
    }

    public async Task<TripDto?> UpdateTripAsync(Guid orgId, Guid id, CreateTripDto dto)
    {
        var trip = await _context.Trips
            .Include(t => t.ItineraryDays)
            .Include(t => t.TripHotels).ThenInclude(th => th.Hotel)
            .Include(t => t.TripVehicles).ThenInclude(tv => tv.Vehicle)
            .Include(t => t.TripVendors).ThenInclude(tv => tv.Vendor)
            .Include(t => t.TripMeals)
            .FirstOrDefaultAsync(t => t.OrganizationId == orgId && t.Id == id);

        if (trip == null) return null;

        trip.TripCode = dto.TripCode.Trim().ToUpper();
        trip.TripName = dto.TripName.Trim();
        trip.ShortDescription = dto.ShortDescription ?? string.Empty;
        trip.Description = dto.Description ?? string.Empty;
        trip.Destination = dto.Destination.Trim();
        trip.StartLocation = dto.StartLocation ?? string.Empty;
        trip.EndLocation = dto.EndLocation ?? string.Empty;
        trip.ViaRoute = dto.ViaRoute;
        trip.StartDate = dto.StartDate;
        trip.EndDate = dto.EndDate;
        trip.DurationDays = dto.DurationDays;
        trip.DurationNights = dto.DurationNights;
        trip.TripType = dto.TripType;
        trip.Visibility = dto.Visibility;
        if (!string.IsNullOrWhiteSpace(dto.CoverImageUrl)) trip.CoverImageUrl = dto.CoverImageUrl;
        trip.BasePrice = dto.BasePrice;
        trip.Currency = dto.Currency ?? "USD";
        
        int difference = dto.TotalCapacity - trip.TotalCapacity;
        trip.TotalCapacity = dto.TotalCapacity;
        trip.AvailableSeats = Math.Max(0, trip.AvailableSeats + difference);
        
        trip.MinimumTravellers = dto.MinimumTravellers;
        trip.MaximumTravellers = dto.MaximumTravellers;
        trip.HostGuide = dto.HostGuide;
        trip.DriverName = dto.DriverName;
        trip.EstimatedCost = dto.EstimatedCost;
        trip.ContactPerson = dto.ContactPerson;
        trip.ContactNumber = dto.ContactNumber;
        trip.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return MapToDto(trip);
    }

    public async Task<bool> PublishTripAsync(Guid orgId, Guid id)
    {
        var trip = await _context.Trips.FirstOrDefaultAsync(t => t.OrganizationId == orgId && t.Id == id);
        if (trip == null) return false;

        trip.Status = TripStatus.RegistrationOpen;
        trip.PublishedAt = DateTime.UtcNow;
        trip.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UnpublishTripAsync(Guid orgId, Guid id)
    {
        var trip = await _context.Trips.FirstOrDefaultAsync(t => t.OrganizationId == orgId && t.Id == id);
        if (trip == null) return false;

        trip.Status = TripStatus.Draft;
        trip.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<TripDto?> DuplicateTripAsync(Guid orgId, Guid id)
    {
        var source = await _context.Trips
            .Include(t => t.ItineraryDays)
            .Include(t => t.TripHotels)
            .Include(t => t.TripVehicles)
            .Include(t => t.TripVendors)
            .Include(t => t.TripMeals)
            .FirstOrDefaultAsync(t => t.OrganizationId == orgId && t.Id == id);

        if (source == null) return null;

        var copy = new Trip
        {
            Id = Guid.NewGuid(),
            OrganizationId = orgId,
            TripCode = $"{source.TripCode}-COPY",
            TripName = $"{source.TripName} (Copy)",
            ShortDescription = source.ShortDescription,
            Description = source.Description,
            Destination = source.Destination,
            StartLocation = source.StartLocation,
            EndLocation = source.EndLocation,
            StartDate = source.StartDate.AddDays(30),
            EndDate = source.EndDate.AddDays(30),
            DurationDays = source.DurationDays,
            DurationNights = source.DurationNights,
            TripType = source.TripType,
            Status = TripStatus.Draft,
            Visibility = source.Visibility,
            CoverImageUrl = source.CoverImageUrl,
            BasePrice = source.BasePrice,
            Currency = source.Currency,
            TotalCapacity = source.TotalCapacity,
            AvailableSeats = source.TotalCapacity,
            MinimumTravellers = source.MinimumTravellers,
            MaximumTravellers = source.MaximumTravellers,
            HostGuide = source.HostGuide,
            ContactPerson = source.ContactPerson,
            ContactNumber = source.ContactNumber,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var d in source.ItineraryDays)
        {
            copy.ItineraryDays.Add(new TripItineraryDay
            {
                Id = Guid.NewGuid(),
                DayNumber = d.DayNumber,
                Date = d.Date?.AddDays(30),
                Title = d.Title,
                Description = d.Description,
                Location = d.Location,
                Activities = d.Activities,
                StartTime = d.StartTime,
                EndTime = d.EndTime,
                Notes = d.Notes
            });
        }

        _context.Trips.Add(copy);
        await _context.SaveChangesAsync();

        return await GetTripByIdAsync(orgId, copy.Id);
    }

    public async Task<bool> DeleteTripAsync(Guid orgId, Guid id)
    {
        var trip = await _context.Trips.FirstOrDefaultAsync(t => t.OrganizationId == orgId && t.Id == id);
        if (trip == null) return false;

        _context.Trips.Remove(trip);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<ItineraryDayDto>> SaveItineraryDaysAsync(Guid orgId, Guid tripId, List<ItineraryDayDto> days)
    {
        var trip = await _context.Trips.Include(t => t.ItineraryDays).FirstOrDefaultAsync(t => t.OrganizationId == orgId && t.Id == tripId);
        if (trip == null) return new();

        _context.TripItineraryDays.RemoveRange(trip.ItineraryDays);

        var newDays = days.Select(d => new TripItineraryDay
        {
            Id = Guid.NewGuid(),
            TripId = tripId,
            DayNumber = d.DayNumber,
            Date = d.Date,
            Title = d.Title,
            Description = d.Description,
            Location = d.Location,
            Activities = d.Activities,
            StartTime = d.StartTime,
            EndTime = d.EndTime,
            Notes = d.Notes
        }).ToList();

        _context.TripItineraryDays.AddRange(newDays);
        await _context.SaveChangesAsync();

        return newDays.Select(d => new ItineraryDayDto(d.Id, d.DayNumber, d.Date, d.Title, d.Description, d.Location, d.Activities, d.StartTime, d.EndTime, d.Notes)).ToList();
    }

    public async Task<List<TripHotelDto>> SaveTripHotelsAsync(Guid orgId, Guid tripId, List<TripHotelDto> hotels)
    {
        var trip = await _context.Trips.Include(t => t.TripHotels).FirstOrDefaultAsync(t => t.OrganizationId == orgId && t.Id == tripId);
        if (trip == null) return new();

        _context.TripHotels.RemoveRange(trip.TripHotels);

        var newHotels = hotels.Select(h => new TripHotel
        {
            Id = Guid.NewGuid(),
            TripId = tripId,
            HotelId = h.HotelId,
            RoomType = h.RoomType,
            CheckIn = h.CheckIn,
            CheckOut = h.CheckOut,
            RoomCount = h.RoomCount,
            Notes = h.Notes
        }).ToList();

        _context.TripHotels.AddRange(newHotels);
        await _context.SaveChangesAsync();

        return await _context.TripHotels
            .Include(th => th.Hotel)
            .Where(th => th.TripId == tripId)
            .Select(th => new TripHotelDto(
                th.Id,
                th.HotelId,
                th.Hotel != null ? th.Hotel.HotelName : "",
                th.Hotel != null ? th.Hotel.Location : "",
                th.RoomType,
                th.CheckIn,
                th.CheckOut,
                th.RoomCount,
                th.Notes))
            .ToListAsync();
    }

    public async Task<List<TripVehicleDto>> SaveTripVehiclesAsync(Guid orgId, Guid tripId, List<TripVehicleDto> vehicles)
    {
        var trip = await _context.Trips.Include(t => t.TripVehicles).FirstOrDefaultAsync(t => t.OrganizationId == orgId && t.Id == tripId);
        if (trip == null) return new();

        _context.TripVehicles.RemoveRange(trip.TripVehicles);

        var newVehicles = vehicles.Select(v => new TripVehicle
        {
            Id = Guid.NewGuid(),
            TripId = tripId,
            VehicleId = v.VehicleId,
            Notes = v.Notes
        }).ToList();

        _context.TripVehicles.AddRange(newVehicles);
        await _context.SaveChangesAsync();

        return await _context.TripVehicles
            .Include(tv => tv.Vehicle)
            .Where(tv => tv.TripId == tripId)
            .Select(tv => new TripVehicleDto(
                tv.Id,
                tv.VehicleId,
                tv.Vehicle != null ? tv.Vehicle.VehicleName : "",
                tv.Vehicle != null ? tv.Vehicle.VehicleType : "",
                tv.Vehicle != null ? tv.Vehicle.Capacity : 0,
                tv.Vehicle != null ? tv.Vehicle.DriverName : null,
                tv.Vehicle != null ? tv.Vehicle.DriverPhone : null,
                tv.Notes))
            .ToListAsync();
    }

    public async Task<List<TripVendorDto>> SaveTripVendorsAsync(Guid orgId, Guid tripId, List<TripVendorDto> vendors)
    {
        var trip = await _context.Trips.Include(t => t.TripVendors).FirstOrDefaultAsync(t => t.OrganizationId == orgId && t.Id == tripId);
        if (trip == null) return new();

        _context.TripVendors.RemoveRange(trip.TripVendors);

        var newVendors = vendors.Select(v => new TripVendor
        {
            Id = Guid.NewGuid(),
            TripId = tripId,
            VendorId = v.VendorId,
            ContractAmount = v.ContractAmount,
            ServiceDescription = v.ServiceDescription
        }).ToList();

        _context.TripVendors.AddRange(newVendors);
        await _context.SaveChangesAsync();

        return await _context.TripVendors
            .Include(tv => tv.Vendor)
            .Where(tv => tv.TripId == tripId)
            .Select(tv => new TripVendorDto(
                tv.Id,
                tv.VendorId,
                tv.Vendor != null ? tv.Vendor.VendorName : "",
                tv.Vendor != null ? tv.Vendor.VendorType : VendorType.Other,
                tv.ContractAmount,
                tv.ServiceDescription))
            .ToListAsync();
    }

    public async Task<List<TripMealDto>> SaveTripMealsAsync(Guid orgId, Guid tripId, List<TripMealDto> meals)
    {
        var trip = await _context.Trips.Include(t => t.TripMeals).FirstOrDefaultAsync(t => t.OrganizationId == orgId && t.Id == tripId);
        if (trip == null) return new();

        _context.TripMeals.RemoveRange(trip.TripMeals);

        var newMeals = meals.Select(m => new TripMeal
        {
            Id = Guid.NewGuid(),
            TripId = tripId,
            MealType = m.MealType,
            MealOption = m.MealOption,
            Description = m.Description,
            DietaryOptions = m.DietaryOptions
        }).ToList();

        _context.TripMeals.AddRange(newMeals);
        await _context.SaveChangesAsync();

        return newMeals.Select(m => new TripMealDto(m.Id, m.MealType, m.MealOption, m.Description, m.DietaryOptions)).ToList();
    }

    private static TripDto MapToDto(Trip t)
    {
        int booked = Math.Max(0, t.TotalCapacity - t.AvailableSeats);
        decimal revenue = booked * t.BasePrice;
        decimal expense = t.EstimatedCost;
        decimal netProfit = revenue - expense;

        return new(
            t.Id,
            t.OrganizationId,
            t.TripCode,
            t.TripName,
            t.ShortDescription,
            t.Description,
            t.Destination,
            t.StartLocation,
            t.EndLocation,
            t.ViaRoute,
            t.StartDate,
            t.EndDate,
            t.DurationDays,
            t.DurationNights,
            t.TripType,
            t.Status,
            t.Visibility,
            t.CoverImageUrl,
            t.BasePrice,
            t.Currency,
            t.TotalCapacity,
            t.AvailableSeats,
            booked,
            t.MinimumTravellers,
            t.MaximumTravellers,
            t.HostGuide,
            t.DriverName,
            expense,
            revenue,
            netProfit,
            t.ContactPerson,
            t.ContactNumber,
            t.CreatedAt,
            t.PublishedAt,
            t.ItineraryDays.OrderBy(d => d.DayNumber).Select(d => new ItineraryDayDto(d.Id, d.DayNumber, d.Date, d.Title, d.Description, d.Location, d.Activities, d.StartTime, d.EndTime, d.Notes)).ToList(),
            t.TripHotels.Select(th => new TripHotelDto(th.Id, th.HotelId, th.Hotel != null ? th.Hotel.HotelName : "", th.Hotel != null ? th.Hotel.Location : "", th.RoomType, th.CheckIn, th.CheckOut, th.RoomCount, th.Notes)).ToList(),
            t.TripVehicles.Select(tv => new TripVehicleDto(tv.Id, tv.VehicleId, tv.Vehicle != null ? tv.Vehicle.VehicleName : "", tv.Vehicle != null ? tv.Vehicle.VehicleType : "", tv.Vehicle != null ? tv.Vehicle.Capacity : 0, tv.Vehicle != null ? tv.Vehicle.DriverName : null, tv.Vehicle != null ? tv.Vehicle.DriverPhone : null, tv.Notes)).ToList(),
            t.TripVendors.Select(tv => new TripVendorDto(tv.Id, tv.VendorId, tv.Vendor != null ? tv.Vendor.VendorName : "", tv.Vendor != null ? tv.Vendor.VendorType : VendorType.Other, tv.ContractAmount, tv.ServiceDescription)).ToList(),
            t.TripMeals.Select(tm => new TripMealDto(tm.Id, tm.MealType, tm.MealOption, tm.Description, tm.DietaryOptions)).ToList()
        );
    }
}
