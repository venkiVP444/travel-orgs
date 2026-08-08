using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelOrgOS.Api.DTOs;
using TravelOrgOS.Infrastructure.Services;

namespace TravelOrgOS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TravellersController : BaseApiController
{
    private readonly ITravellerService _travellerService;

    public TravellersController(ITravellerService travellerService)
    {
        _travellerService = travellerService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTravellers([FromQuery] string? search)
    {
        var result = await _travellerService.GetTravellersAsync(GetOrgId(), search);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTraveller(Guid id)
    {
        var result = await _travellerService.GetTravellerByIdAsync(GetOrgId(), id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTraveller([FromBody] CreateTravellerDto dto)
    {
        var result = await _travellerService.CreateTravellerAsync(GetOrgId(), dto);
        return CreatedAtAction(nameof(GetTraveller), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTraveller(Guid id, [FromBody] CreateTravellerDto dto)
    {
        var result = await _travellerService.UpdateTravellerAsync(GetOrgId(), id, dto);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTraveller(Guid id)
    {
        var result = await _travellerService.DeleteTravellerAsync(GetOrgId(), id);
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpPost("import")]
    public async Task<IActionResult> ImportCsv([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { message = "No CSV file uploaded." });
        }

        using var stream = file.OpenReadStream();
        var result = await _travellerService.ImportCsvAsync(GetOrgId(), stream);
        return Ok(result);
    }

    [HttpGet("import/template")]
    public IActionResult DownloadTemplate()
    {
        var bytes = _travellerService.GenerateCsvTemplate();
        return File(bytes, "text/csv", "Traveller_Import_Template.csv");
    }
}
