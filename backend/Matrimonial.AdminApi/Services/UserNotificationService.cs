using Matrimonial.AdminApi.Data;
using Matrimonial.AdminApi.DTOs.UserPanel;
using Microsoft.EntityFrameworkCore;

namespace Matrimonial.AdminApi.Services;

public interface IUserNotificationService
{
    Task<List<NotificationDto>> GetAllAsync(Guid userId);
    Task<int> GetUnreadCountAsync(Guid userId);
    Task MarkReadAsync(Guid userId, Guid notificationId);
    Task MarkAllReadAsync(Guid userId);
}

public class UserNotificationService : IUserNotificationService
{
    private readonly ApplicationDbContext _context;

    public UserNotificationService(ApplicationDbContext context) => _context = context;

    public async Task<List<NotificationDto>> GetAllAsync(Guid userId) =>
        await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedOn)
            .Take(50)
            .Select(n => new NotificationDto
            {
                NotificationId = n.NotificationId,
                Title = n.Title,
                Message = n.MessageText,
                RelatedUserId = n.RelatedUserId,
                IsRead = n.IsRead,
                CreatedOn = n.CreatedOn
            }).ToListAsync();

    public async Task<int> GetUnreadCountAsync(Guid userId) =>
        await _context.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead);

    public async Task MarkReadAsync(Guid userId, Guid notificationId)
    {
        var n = await _context.Notifications.FirstOrDefaultAsync(x =>
            x.NotificationId == notificationId && x.UserId == userId);
        if (n == null) return;
        n.IsRead = true;
        await _context.SaveChangesAsync();
    }

    public async Task MarkAllReadAsync(Guid userId)
    {
        var items = await _context.Notifications.Where(n => n.UserId == userId && !n.IsRead).ToListAsync();
        foreach (var n in items) n.IsRead = true;
        await _context.SaveChangesAsync();
    }
}
