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

public interface ITeamService
{
    Task<List<TeamMemberDto>> GetTeamMembersAsync(Guid orgId);
    Task<TeamMemberDto?> GetMemberByIdAsync(Guid orgId, Guid id);
    Task<string> InviteMemberAsync(Guid orgId, InviteTeamMemberDto dto);
    Task<bool> AcceptInviteAsync(AcceptInviteDto dto);
    Task<TeamMemberDto?> ToggleMemberStatusAsync(Guid orgId, Guid id);
    Task<TeamMemberDto?> ChangeRoleAsync(Guid orgId, Guid id, UserRole role);
}

public class TeamService : ITeamService
{
    private readonly TravelOrgOSDbContext _context;

    public TeamService(TravelOrgOSDbContext context)
    {
        _context = context;
    }

    public async Task<List<TeamMemberDto>> GetTeamMembersAsync(Guid orgId)
    {
        var members = await _context.OrganizationUsers
            .Where(u => u.OrganizationId == orgId)
            .OrderBy(u => u.FullName)
            .ToListAsync();

        return members.Select(MapToDto).ToList();
    }

    public async Task<TeamMemberDto?> GetMemberByIdAsync(Guid orgId, Guid id)
    {
        var member = await _context.OrganizationUsers.FirstOrDefaultAsync(u => u.OrganizationId == orgId && u.Id == id);
        return member == null ? null : MapToDto(member);
    }

    public async Task<string> InviteMemberAsync(Guid orgId, InviteTeamMemberDto dto)
    {
        var exists = await _context.OrganizationUsers.AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower());
        if (exists)
        {
            throw new InvalidOperationException("A user with this email is already registered.");
        }

        var token = Guid.NewGuid().ToString("N");
        var user = new OrganizationUser
        {
            Id = Guid.NewGuid(),
            OrganizationId = orgId,
            FullName = dto.FullName,
            Email = dto.Email,
            PasswordHash = $"INVITE_TOKEN:{token}", // Stateless invite token store
            Role = dto.Role,
            Status = false, // Inactive until accepted
            CreatedAt = DateTime.UtcNow
        };

        _context.OrganizationUsers.Add(user);
        await _context.SaveChangesAsync();

        // Renders mock registration links for onboarding flow
        return $"http://localhost:4400/login?inviteToken={token}";
    }

    public async Task<bool> AcceptInviteAsync(AcceptInviteDto dto)
    {
        var tokenPrefix = $"INVITE_TOKEN:{dto.Token}";
        var user = await _context.OrganizationUsers.FirstOrDefaultAsync(u => u.PasswordHash == tokenPrefix);
        if (user == null)
        {
            return false;
        }

        // Activate user and set password
        user.PasswordHash = dto.Password;
        user.Status = true;
        user.LastLoginAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<TeamMemberDto?> ToggleMemberStatusAsync(Guid orgId, Guid id)
    {
        var user = await _context.OrganizationUsers.FirstOrDefaultAsync(u => u.OrganizationId == orgId && u.Id == id);
        if (user == null) return null;

        user.Status = !user.Status;
        await _context.SaveChangesAsync();
        return MapToDto(user);
    }

    public async Task<TeamMemberDto?> ChangeRoleAsync(Guid orgId, Guid id, UserRole role)
    {
        var user = await _context.OrganizationUsers.FirstOrDefaultAsync(u => u.OrganizationId == orgId && u.Id == id);
        if (user == null) return null;

        // Block changing Owner role through basic controls
        if (user.Role == UserRole.OrganizationOwner && role != UserRole.OrganizationOwner)
        {
            throw new InvalidOperationException("The Organization Owner role cannot be changed.");
        }

        user.Role = role;
        await _context.SaveChangesAsync();
        return MapToDto(user);
    }

    private static TeamMemberDto MapToDto(OrganizationUser u) => new(
        u.Id,
        u.OrganizationId,
        u.FullName,
        u.Email,
        u.Role,
        u.Status,
        u.CreatedAt,
        u.LastLoginAt
    );
}
