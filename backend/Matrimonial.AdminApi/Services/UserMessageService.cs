using Matrimonial.AdminApi.Data;
using Matrimonial.AdminApi.DTOs.UserPanel;
using Microsoft.EntityFrameworkCore;

namespace Matrimonial.AdminApi.Services;

public interface IUserMessageService
{
    Task<List<ConversationDto>> GetConversationsAsync(Guid userId);
    Task<List<MessageDto>> GetMessagesAsync(Guid userId, Guid otherUserId);
    Task<MessageDto?> SendMessageAsync(Guid senderId, SendMessageRequest request);
    Task MarkReadAsync(Guid userId, Guid otherUserId);
}

public class UserMessageService : IUserMessageService
{
    private readonly ApplicationDbContext _context;

    public UserMessageService(ApplicationDbContext context) => _context = context;

    public async Task<List<ConversationDto>> GetConversationsAsync(Guid userId)
    {
        var messages = await _context.Messages
            .Include(m => m.Sender).ThenInclude(s => s.Photos)
            .Include(m => m.Receiver).ThenInclude(r => r.Photos)
            .Where(m => m.SenderUserId == userId || m.ReceiverUserId == userId)
            .OrderByDescending(m => m.SentOn)
            .ToListAsync();

        return messages
            .GroupBy(m => m.SenderUserId == userId ? m.ReceiverUserId : m.SenderUserId)
            .Select(g =>
            {
                var other = g.First().SenderUserId == userId ? g.First().Receiver : g.First().Sender;
                var last = g.First();
                return new ConversationDto
                {
                    UserId = other.UserId,
                    Name = $"{other.FirstName} {other.LastName}",
                    PhotoUrl = other.Photos.FirstOrDefault(p => p.IsApproved && p.IsPrimary)?.PhotoUrl
                        ?? other.Photos.FirstOrDefault(p => p.IsApproved)?.PhotoUrl,
                    LastMessage = last.MessageText,
                    LastMessageOn = last.SentOn,
                    UnreadCount = g.Count(m => m.ReceiverUserId == userId && !m.IsRead)
                };
            }).ToList();
    }

    public async Task<List<MessageDto>> GetMessagesAsync(Guid userId, Guid otherUserId)
    {
        var messages = await _context.Messages
            .Include(m => m.Sender)
            .Where(m =>
                (m.SenderUserId == userId && m.ReceiverUserId == otherUserId) ||
                (m.SenderUserId == otherUserId && m.ReceiverUserId == userId))
            .OrderBy(m => m.SentOn)
            .ToListAsync();

        return messages.Select(m => new MessageDto
        {
            MessageId = m.MessageId,
            SenderUserId = m.SenderUserId,
            ReceiverUserId = m.ReceiverUserId,
            SenderName = $"{m.Sender.FirstName} {m.Sender.LastName}",
            Message = m.MessageText,
            IsRead = m.IsRead,
            SentOn = m.SentOn
        }).ToList();
    }

    public async Task<MessageDto?> SendMessageAsync(Guid senderId, SendMessageRequest request)
    {
        var canMessage = await _context.Matches.AnyAsync(m =>
            (m.UserId1 == senderId && m.UserId2 == request.ReceiverUserId) ||
            (m.UserId1 == request.ReceiverUserId && m.UserId2 == senderId));
        if (!canMessage) throw new InvalidOperationException("You can only message matched users.");

        var sender = await _context.Users.FindAsync(senderId);
        if (sender == null) return null;

        var msg = new Entities.Message
        {
            MessageId = Guid.NewGuid(),
            SenderUserId = senderId,
            ReceiverUserId = request.ReceiverUserId,
            MessageText = request.Message,
            IsRead = false,
            SentOn = DateTime.UtcNow
        };
        _context.Messages.Add(msg);

        _context.Notifications.Add(new Entities.Notification
        {
            NotificationId = Guid.NewGuid(),
            UserId = request.ReceiverUserId,
            Title = "New Message",
            MessageText = $"New message from {sender.FirstName}",
            RelatedUserId = senderId,
            CreatedOn = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        return new MessageDto
        {
            MessageId = msg.MessageId,
            SenderUserId = senderId,
            ReceiverUserId = request.ReceiverUserId,
            SenderName = $"{sender.FirstName} {sender.LastName}",
            Message = msg.MessageText,
            IsRead = false,
            SentOn = msg.SentOn
        };
    }

    public async Task MarkReadAsync(Guid userId, Guid otherUserId)
    {
        var unread = await _context.Messages
            .Where(m => m.SenderUserId == otherUserId && m.ReceiverUserId == userId && !m.IsRead)
            .ToListAsync();
        foreach (var m in unread) m.IsRead = true;
        await _context.SaveChangesAsync();
    }
}
