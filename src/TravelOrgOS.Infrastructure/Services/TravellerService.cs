using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using TravelOrgOS.Api.DTOs;
using TravelOrgOS.Domain.Entities;
using TravelOrgOS.Infrastructure.Data;

namespace TravelOrgOS.Infrastructure.Services;

public interface ITravellerService
{
    Task<List<TravellerDto>> GetTravellersAsync(Guid orgId, string? search = null);
    Task<TravellerDto?> GetTravellerByIdAsync(Guid orgId, Guid id);
    Task<TravellerDto> CreateTravellerAsync(Guid orgId, CreateTravellerDto dto);
    Task<TravellerDto?> UpdateTravellerAsync(Guid orgId, Guid id, CreateTravellerDto dto);
    Task<bool> DeleteTravellerAsync(Guid orgId, Guid id);
    Task<CsvImportSummaryDto> ImportCsvAsync(Guid orgId, Stream csvStream);
    byte[] GenerateCsvTemplate();
}

public class TravellerService : ITravellerService
{
    private readonly TravelOrgOSDbContext _context;

    public TravellerService(TravelOrgOSDbContext context)
    {
        _context = context;
    }

    public async Task<List<TravellerDto>> GetTravellersAsync(Guid orgId, string? search = null)
    {
        var query = _context.Travellers.Where(t => t.OrganizationId == orgId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.ToLower();
            query = query.Where(t =>
                t.FirstName.ToLower().Contains(s) ||
                t.LastName.ToLower().Contains(s) ||
                t.Email.ToLower().Contains(s) ||
                t.MobileNumber.Contains(s) ||
                (t.PassportNumber != null && t.PassportNumber.ToLower().Contains(s)));
        }

        var travellers = await query.OrderByDescending(t => t.CreatedAt).ToListAsync();

        return travellers.Select(t => MapToDto(t)).ToList();
    }

    public async Task<TravellerDto?> GetTravellerByIdAsync(Guid orgId, Guid id)
    {
        var traveller = await _context.Travellers
            .FirstOrDefaultAsync(t => t.OrganizationId == orgId && t.Id == id);

        return traveller == null ? null : MapToDto(traveller);
    }

    public async Task<TravellerDto> CreateTravellerAsync(Guid orgId, CreateTravellerDto dto)
    {
        var entity = new Traveller
        {
            Id = Guid.NewGuid(),
            OrganizationId = orgId,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            MobileNumber = dto.MobileNumber,
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender,
            Nationality = dto.Nationality,
            PassportNumber = dto.PassportNumber,
            PassportExpiry = dto.PassportExpiry,
            EmergencyContactName = dto.EmergencyContactName,
            EmergencyContactNumber = dto.EmergencyContactNumber,
            Address = dto.Address,
            City = dto.City,
            Country = dto.Country,
            Notes = dto.Notes,
            Status = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Travellers.Add(entity);
        await _context.SaveChangesAsync();

        return MapToDto(entity);
    }

    public async Task<TravellerDto?> UpdateTravellerAsync(Guid orgId, Guid id, CreateTravellerDto dto)
    {
        var entity = await _context.Travellers
            .FirstOrDefaultAsync(t => t.OrganizationId == orgId && t.Id == id);

        if (entity == null) return null;

        entity.FirstName = dto.FirstName;
        entity.LastName = dto.LastName;
        entity.Email = dto.Email;
        entity.MobileNumber = dto.MobileNumber;
        entity.DateOfBirth = dto.DateOfBirth;
        entity.Gender = dto.Gender;
        entity.Nationality = dto.Nationality;
        entity.PassportNumber = dto.PassportNumber;
        entity.PassportExpiry = dto.PassportExpiry;
        entity.EmergencyContactName = dto.EmergencyContactName;
        entity.EmergencyContactNumber = dto.EmergencyContactNumber;
        entity.Address = dto.Address;
        entity.City = dto.City;
        entity.Country = dto.Country;
        entity.Notes = dto.Notes;
        entity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapToDto(entity);
    }

    public async Task<bool> DeleteTravellerAsync(Guid orgId, Guid id)
    {
        var entity = await _context.Travellers
            .FirstOrDefaultAsync(t => t.OrganizationId == orgId && t.Id == id);

        if (entity == null) return false;

        _context.Travellers.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<CsvImportSummaryDto> ImportCsvAsync(Guid orgId, Stream csvStream)
    {
        using var reader = new StreamReader(csvStream);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HeaderValidated = null,
            MissingFieldFound = null
        });

        var records = csv.GetRecords<TravellerCsvRowDto>().ToList();
        var total = records.Count;
        var success = 0;
        var failed = 0;
        var duplicate = 0;
        var errors = new List<string>();

        var existingEmails = await _context.Travellers
            .Where(t => t.OrganizationId == orgId)
            .Select(t => t.Email.ToLower())
            .ToListAsync();

        var newTravellers = new List<Traveller>();

        for (int i = 0; i < records.Count; i++)
        {
            var row = records[i];
            int rowNum = i + 2; // header is row 1

            if (string.IsNullOrWhiteSpace(row.FirstName) || string.IsNullOrWhiteSpace(row.LastName) || string.IsNullOrWhiteSpace(row.Email))
            {
                failed++;
                errors.Add($"Row {rowNum}: Missing required fields (FirstName, LastName, or Email).");
                continue;
            }

            if (existingEmails.Contains(row.Email.ToLower()))
            {
                duplicate++;
                errors.Add($"Row {rowNum}: Traveller with email '{row.Email}' already exists.");
                continue;
            }

            DateTime? dob = null;
            if (!string.IsNullOrWhiteSpace(row.DateOfBirth) && DateTime.TryParse(row.DateOfBirth, out var parsedDob))
            {
                dob = parsedDob;
            }

            var entity = new Traveller
            {
                Id = Guid.NewGuid(),
                OrganizationId = orgId,
                FirstName = row.FirstName.Trim(),
                LastName = row.LastName.Trim(),
                Email = row.Email.Trim(),
                MobileNumber = row.MobileNumber?.Trim() ?? "+1-555-0000",
                DateOfBirth = dob,
                Gender = row.Gender?.Trim(),
                Nationality = row.Nationality?.Trim(),
                PassportNumber = row.PassportNumber?.Trim(),
                EmergencyContactName = row.EmergencyContactName?.Trim(),
                EmergencyContactNumber = row.EmergencyContactNumber?.Trim(),
                City = row.City?.Trim(),
                Country = row.Country?.Trim(),
                Status = true,
                CreatedAt = DateTime.UtcNow
            };

            newTravellers.Add(entity);
            existingEmails.Add(row.Email.ToLower());
            success++;
        }

        if (newTravellers.Any())
        {
            _context.Travellers.AddRange(newTravellers);
            await _context.SaveChangesAsync();
        }

        return new CsvImportSummaryDto(total, success, failed, duplicate, errors);
    }

    public byte[] GenerateCsvTemplate()
    {
        var csvContent = "FirstName,LastName,Email,MobileNumber,DateOfBirth,Gender,Nationality,PassportNumber,EmergencyContactName,EmergencyContactNumber,City,Country\n" +
                         "John,Doe,john.doe@example.com,+1-555-0199,1990-05-15,Male,American,US1234567,Jane Doe,+1-555-0198,San Francisco,USA\n" +
                         "Maria,Garcia,maria.g@example.com,+1-555-0200,1993-11-20,Female,Spanish,ES7654321,Carlos Garcia,+1-555-0201,Madrid,Spain\n";
        return System.Text.Encoding.UTF8.GetBytes(csvContent);
    }

    private static TravellerDto MapToDto(Traveller t) => new(
        t.Id,
        t.OrganizationId,
        t.FirstName,
        t.LastName,
        t.Email,
        t.MobileNumber,
        t.DateOfBirth,
        t.Gender,
        t.Nationality,
        t.PassportNumber,
        t.PassportExpiry,
        t.EmergencyContactName,
        t.EmergencyContactNumber,
        t.Address,
        t.City,
        t.Country,
        t.Notes,
        t.Status,
        t.CreatedAt
    );
}
