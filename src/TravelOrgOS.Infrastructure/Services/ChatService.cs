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

public interface IChatService
{
    Task<List<ChatMessageDto>> GetMessagesAsync(Guid orgId, Guid? tripId = null, Guid? bookingId = null);
    Task<ChatMessageDto> CreateMessageAsync(Guid orgId, CreateChatMessageDto dto);
}

public class ChatService : IChatService
{
    private readonly TravelOrgOSDbContext _context;

    public ChatService(TravelOrgOSDbContext context)
    {
        _context = context;
    }

    public async Task<List<ChatMessageDto>> GetMessagesAsync(Guid orgId, Guid? tripId = null, Guid? bookingId = null)
    {
        var query = _context.ChatMessages.Where(m => m.OrganizationId == orgId);

        if (tripId.HasValue)
        {
            query = query.Where(m => m.TripId == tripId.Value);
        }
        if (bookingId.HasValue)
        {
            query = query.Where(m => m.BookingId == bookingId.Value);
        }

        var messages = await query.OrderBy(m => m.Timestamp).ToListAsync();
        return messages.Select(MapToDto).ToList();
    }

    public async Task<ChatMessageDto> CreateMessageAsync(Guid orgId, CreateChatMessageDto dto)
    {
        var message = new ChatMessage
        {
            Id = Guid.NewGuid(),
            OrganizationId = orgId,
            TripId = dto.TripId,
            BookingId = dto.BookingId,
            SenderName = dto.SenderName,
            MessageText = dto.MessageText,
            MessageType = dto.MessageType,
            AttachmentUrl = dto.AttachmentUrl,
            Timestamp = DateTime.UtcNow
        };

        _context.ChatMessages.Add(message);
        await _context.SaveChangesAsync();
        return MapToDto(message);
    }

    private static ChatMessageDto MapToDto(ChatMessage m) => new(
        m.Id,
        m.OrganizationId,
        m.TripId,
        m.BookingId,
        m.SenderName,
        m.MessageText,
        m.MessageType,
        m.AttachmentUrl,
        m.Timestamp
    );
}
