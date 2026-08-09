using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TravelOrgOS.Api.Authorization;
using TravelOrgOS.Api.DTOs;
using TravelOrgOS.Domain.Enums;
using TravelOrgOS.Infrastructure.Services;

namespace TravelOrgOS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripsController : BaseApiController
{
    private readonly ITripService _tripService;

    public TripsController(ITripService tripService)
    {
        _tripService = tripService;
    }

    [HttpGet]
    [RequiresPermission("Trip.View")]
    public async Task<IActionResult> GetTrips([FromQuery] string? search, [FromQuery] TripStatus? status, [FromQuery] bool publicOnly = false)
    {
        var trips = await _tripService.GetTripsAsync(GetOrgId(), search, status, publicOnly);
        return Ok(trips);
    }

    [HttpGet("{id:guid}")]
    [RequiresPermission("Trip.View")]
    public async Task<IActionResult> GetTrip(Guid id)
    {
        var trip = await _tripService.GetTripByIdAsync(GetOrgId(), id);
        if (trip == null) return NotFound();
        return Ok(trip);
    }

    [AllowAnonymous]
    [HttpGet("portal/{orgSlug}/{tripId:guid}")]
    public async Task<IActionResult> GetTripForPortal(string orgSlug, Guid tripId)
    {
        var trip = await _tripService.GetTripBySlugOrIdForPortalAsync(orgSlug, tripId);
        if (trip == null) return NotFound();
        return Ok(trip);
    }

    [AllowAnonymous]
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
    [RequiresPermission("Trip.Create")]
    public async Task<IActionResult> CreateTrip([FromBody] CreateTripDto dto)
    {
        var trip = await _tripService.CreateTripAsync(GetOrgId(), dto);
        return CreatedAtAction(nameof(GetTrip), new { id = trip.Id }, trip);
    }

    [HttpPut("{id:guid}")]
    [RequiresPermission("Trip.Edit")]
    public async Task<IActionResult> UpdateTrip(Guid id, [FromBody] CreateTripDto dto)
    {
        var trip = await _tripService.UpdateTripAsync(GetOrgId(), id, dto);
        if (trip == null) return NotFound();
        return Ok(trip);
    }

    [HttpPost("{id:guid}/publish")]
    [RequiresPermission("Trip.Publish")]
    public async Task<IActionResult> PublishTrip(Guid id)
    {
        try
        {
            var result = await _tripService.PublishTripAsync(GetOrgId(), id);
            if (!result) return NotFound();
            return Ok(new { message = "Trip published successfully!" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id:guid}/unpublish")]
    [RequiresPermission("Trip.Publish")]
    public async Task<IActionResult> UnpublishTrip(Guid id)
    {
        var result = await _tripService.UnpublishTripAsync(GetOrgId(), id);
        if (!result) return NotFound();
        return Ok(new { message = "Trip saved as draft." });
    }

    [HttpPost("{id:guid}/duplicate")]
    [RequiresPermission("Trip.Create")]
    public async Task<IActionResult> DuplicateTrip(Guid id)
    {
        var trip = await _tripService.DuplicateTripAsync(GetOrgId(), id);
        if (trip == null) return NotFound();
        return Ok(trip);
    }

    [HttpDelete("{id:guid}")]
    [RequiresPermission("Trip.Edit")]
    public async Task<IActionResult> DeleteTrip(Guid id)
    {
        var result = await _tripService.DeleteTripAsync(GetOrgId(), id);
        if (!result) return NotFound();
        return NoContent();
    }

    // Builder Step Endpoints
    [HttpPost("{id:guid}/itinerary")]
    [RequiresPermission("Trip.Edit")]
    public async Task<IActionResult> SaveItinerary(Guid id, [FromBody] List<ItineraryDayDto> days)
    {
        var result = await _tripService.SaveItineraryDaysAsync(GetOrgId(), id, days);
        return Ok(result);
    }

    [HttpPost("{id:guid}/hotels")]
    [RequiresPermission("Trip.Edit")]
    public async Task<IActionResult> SaveHotels(Guid id, [FromBody] List<TripHotelDto> hotels)
    {
        var result = await _tripService.SaveTripHotelsAsync(GetOrgId(), id, hotels);
        return Ok(result);
    }

    [HttpPost("{id:guid}/vehicles")]
    [RequiresPermission("Trip.Edit")]
    public async Task<IActionResult> SaveVehicles(Guid id, [FromBody] List<TripVehicleDto> vehicles)
    {
        try
        {
            var result = await _tripService.SaveTripVehiclesAsync(GetOrgId(), id, vehicles);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("{id:guid}/vendors")]
    [RequiresPermission("Trip.Edit")]
    public async Task<IActionResult> SaveVendors(Guid id, [FromBody] List<TripVendorDto> vendors)
    {
        var result = await _tripService.SaveTripVendorsAsync(GetOrgId(), id, vendors);
        return Ok(result);
    }

    [HttpPost("{id:guid}/meals")]
    [RequiresPermission("Trip.Edit")]
    public async Task<IActionResult> SaveMeals(Guid id, [FromBody] List<TripMealDto> meals)
    {
        var result = await _tripService.SaveTripMealsAsync(GetOrgId(), id, meals);
        return Ok(result);
    }

    [HttpPost("{id:guid}/guides")]
    [RequiresPermission("Trip.Edit")]
    public async Task<IActionResult> SaveGuides(Guid id, [FromBody] List<TripGuideDto> guides)
    {
        try
        {
            var result = await _tripService.SaveTripGuidesAsync(GetOrgId(), id, guides);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}

