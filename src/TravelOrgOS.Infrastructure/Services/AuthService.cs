using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TravelOrgOS.Api.DTOs;
using TravelOrgOS.Domain.Entities;
using TravelOrgOS.Infrastructure.Data;

namespace TravelOrgOS.Infrastructure.Services;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
}

public class AuthService : IAuthService
{
    private readonly TravelOrgOSDbContext _context;
    private readonly IConfiguration _config;

    public AuthService(TravelOrgOSDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
    {
        var user = await _context.OrganizationUsers
            .Include(u => u.Organization)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

        if (user == null || !user.Status)
        {
            return null;
        }

        // Development mode demo password validation ('Demo@123' or matching hash)
        bool isValidPassword = request.Password == "Demo@123" || user.PasswordHash == request.Password;

        if (!isValidPassword)
        {
            return null;
        }

        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var token = GenerateJwtToken(user);

        return new LoginResponseDto(
            Token: token,
            UserId: user.Id,
            FullName: user.FullName,
            Email: user.Email,
            Role: user.Role,
            OrganizationId: user.OrganizationId,
            OrganizationName: user.Organization?.Name,
            OrganizationSlug: user.Organization?.Slug,
            PrimaryColor: user.Organization?.PrimaryColor,
            SecondaryColor: user.Organization?.SecondaryColor,
            LogoUrl: user.Organization?.LogoUrl
        );
    }

    private string GenerateJwtToken(OrganizationUser user)
    {
        var secret = _config["Jwt:Secret"] ?? "SuperSecretTravelOrgOSJwtKey2026WithMaximumSecurityLengthRequiredForHS256!";
        var issuer = _config["Jwt:Issuer"] ?? "TravelOrgOS.Api";
        var audience = _config["Jwt:Audience"] ?? "TravelOrgOS.Web";

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        if (user.OrganizationId.HasValue)
        {
            claims.Add(new Claim("OrganizationId", user.OrganizationId.Value.ToString()));
        }

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
