using Microsoft.AspNetCore.Mvc;
using TravelOrgOS.Api.DTOs;
using TravelOrgOS.Domain.Enums;
using TravelOrgOS.Infrastructure.Services;

namespace TravelOrgOS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripsController : ControllerBase
{
    private readonly ITripService _tripService;

    public TripsController(ITripService tripService)
    {
        _tripService = tripService;
    }

    private Guid GetOrgId()
    {
        var claim = User.FindFirst("OrganizationId")?.Value;
        if (Guid.TryParse(claim, out var orgId)) return orgId;
        return Guid.Parse("11111111-1111-1111-1111-111111111111");
    }

    [HttpGet]
    public async Task<IActionResult> GetTrips([FromQuery] string? search, [FromQuery] TripStatus? status, [FromQuery] bool publicOnly = false)
    {
        var trips = await _tripService.GetTripsAsync(GetOrgId(), search, status, publicOnly);
        return Ok(trips);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTrip(Guid id)
    {
        var trip = await _tripService.GetTripByIdAsync(GetOrgId(), id);
        if (trip == null) return NotFound();
        return Ok(trip);
    }

    [HttpGet("portal/{orgSlug}/{tripId:guid}")]
    public async Task<IActionResult> GetTripForPortal(string orgSlug, Guid tripId)
    {
        var trip = await _tripService.GetTripBySlugOrIdForPortalAsync(orgSlug, tripId);
        if (trip == null) return NotFound();
        return Ok(trip);
    }

    [HttpGet("portal/{orgSlug}")]
    public async Task<IActionResult> GetTripsForPortal(string orgSlug)
    {
        var masterDataService = HttpContext.RequestServices.GetRequiredService<IMasterDataService>();
        var org = await masterDataService.GetOrganizationBySlugAsync(orgSlug);
        if (org == null) return NotFound();

        var trips = await _tripService.GetTripsAsync(org.Id, publicOnly: true);
        return Ok(new { Organization = org, Trips = trips });
    }

    [HttpPost]
    public async Task<IActionResult> CreateTrip([FromBody] CreateTripDto dto)
    {
        var trip = await _tripService.CreateTripAsync(GetOrgId(), dto);
        return CreatedAtAction(nameof(GetTrip), new { id = trip.Id }, trip);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTrip(Guid id, [FromBody] CreateTripDto dto)
    {
        var trip = await _tripService.UpdateTripAsync(GetOrgId(), id, dto);
        if (trip == null) return NotFound();
        return Ok(trip);
    }

    [HttpPost("{id:guid}/publish")]
    public async Task<IActionResult> PublishTrip(Guid id)
    {
        var result = await _tripService.PublishTripAsync(GetOrgId(), id);
        if (!result) return NotFound();
        return Ok(new { message = "Trip published successfully!" });
    }

    [HttpPost("{id:guid}/unpublish")]
    public async Task<IActionResult> UnpublishTrip(Guid id)
    {
        var result = await _tripService.UnpublishTripAsync(GetOrgId(), id);
        if (!result) return NotFound();
        return Ok(new { message = "Trip saved as draft." });
    }

    [HttpPost("{id:guid}/duplicate")]
    public async Task<IActionResult> DuplicateTrip(Guid id)
    {
        var trip = await _tripService.DuplicateTripAsync(GetOrgId(), id);
        if (trip == null) return NotFound();
        return Ok(trip);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTrip(Guid id)
    {
        var result = await _tripService.DeleteTripAsync(GetOrgId(), id);
        if (!result) return NotFound();
        return NoContent();
    }

    // Builder Step Endpoints
    [HttpPost("{id:guid}/itinerary")]
    public async Task<IActionResult> SaveItinerary(Guid id, [FromBody] List<ItineraryDayDto> days)
    {
        var result = await _tripService.SaveItineraryDaysAsync(GetOrgId(), id, days);
        return Ok(result);
    }

    [HttpPost("{id:guid}/hotels")]
    public async Task<IActionResult> SaveHotels(Guid id, [FromBody] List<TripHotelDto> hotels)
    {
        var result = await _tripService.SaveTripHotelsAsync(GetOrgId(), id, hotels);
        return Ok(result);
    }

    [HttpPost("{id:guid}/vehicles")]
    public async Task<IActionResult> SaveVehicles(Guid id, [FromBody] List<TripVehicleDto> vehicles)
    {
        var result = await _tripService.SaveTripVehiclesAsync(GetOrgId(), id, vehicles);
        return Ok(result);
    }

    [HttpPost("{id:guid}/vendors")]
    public async Task<IActionResult> SaveVendors(Guid id, [FromBody] List<TripVendorDto> vendors)
    {
        var result = await _tripService.SaveTripVendorsAsync(GetOrgId(), id, vendors);
        return Ok(result);
    }

    [HttpPost("{id:guid}/meals")]
    public async Task<IActionResult> SaveMeals(Guid id, [FromBody] List<TripMealDto> meals)
    {
        var result = await _tripService.SaveTripMealsAsync(GetOrgId(), id, meals);
        return Ok(result);
    }
}
