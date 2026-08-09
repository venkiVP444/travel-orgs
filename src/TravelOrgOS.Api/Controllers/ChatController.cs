using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TravelOrgOS.Api.Authorization;
using TravelOrgOS.Api.DTOs;
using TravelOrgOS.Infrastructure.Services;

namespace TravelOrgOS.Api.Controllers;

[Route("api/[controller]")]
public class ChatController : BaseApiController
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpGet]
    [RequiresPermission("Trip.View")]
    public async Task<IActionResult> GetMessages([FromQuery] Guid? tripId, [FromQuery] Guid? bookingId)
    {
        var orgId = GetOrgId();
        var messages = await _chatService.GetMessagesAsync(orgId, tripId, bookingId);
        return Ok(messages);
    }

    [HttpPost]
    [RequiresPermission("Trip.View")]
    public async Task<IActionResult> CreateMessage([FromBody] CreateChatMessageDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var orgId = GetOrgId();
        var msg = await _chatService.CreateMessageAsync(orgId, dto);
        return Ok(msg);
    }
}
