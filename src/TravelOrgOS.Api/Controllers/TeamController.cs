using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelOrgOS.Api.Authorization;
using TravelOrgOS.Api.DTOs;
using TravelOrgOS.Domain.Enums;
using TravelOrgOS.Infrastructure.Services;

namespace TravelOrgOS.Api.Controllers;

[Route("api/[controller]")]
public class TeamController : BaseApiController
{
    private readonly ITeamService _teamService;

    public TeamController(ITeamService teamService)
    {
        _teamService = teamService;
    }

    [HttpGet]
    [RequiresPermission("Team.View")]
    public async Task<IActionResult> GetTeam()
    {
        var orgId = GetOrgId();
        var members = await _teamService.GetTeamMembersAsync(orgId);
        return Ok(members);
    }

    [HttpGet("{id}")]
    [RequiresPermission("Team.View")]
    public async Task<IActionResult> GetMemberById(Guid id)
    {
        var orgId = GetOrgId();
        var member = await _teamService.GetMemberByIdAsync(orgId, id);
        if (member == null) return NotFound("Member not found.");
        return Ok(member);
    }

    [HttpPost("invite")]
    [RequiresPermission("Team.Manage")]
    public async Task<IActionResult> InviteMember([FromBody] InviteTeamMemberDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var orgId = GetOrgId();
        try
        {
            var inviteLink = await _teamService.InviteMemberAsync(orgId, dto);
            return Ok(new { InviteLink = inviteLink });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [AllowAnonymous]
    [HttpPost("accept")]
    public async Task<IActionResult> AcceptInvitation([FromBody] AcceptInviteDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var success = await _teamService.AcceptInviteAsync(dto);
        if (!success) return BadRequest("Invalid or expired invitation token.");
        return Ok(new { Message = "Invitation accepted. User activated." });
    }

    [HttpPost("{id}/toggle-status")]
    [RequiresPermission("Team.Manage")]
    public async Task<IActionResult> ToggleStatus(Guid id)
    {
        var orgId = GetOrgId();
        var member = await _teamService.ToggleMemberStatusAsync(orgId, id);
        if (member == null) return NotFound("Member not found.");
        return Ok(member);
    }

    [HttpPost("{id}/role")]
    [RequiresPermission("Team.Manage")]
    public async Task<IActionResult> ChangeRole(Guid id, [FromBody] UserRole role)
    {
        var orgId = GetOrgId();
        try
        {
            var member = await _teamService.ChangeRoleAsync(orgId, id, role);
            if (member == null) return NotFound("Member not found.");
            return Ok(member);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
