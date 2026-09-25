using InventoryManagement.Core.Entities;
using InventoryManagement.Core.Enums;
using InventoryManagement.Core.Interfaces.Services;
using InventoryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Services.Services;

public class NotificationService : INotificationService
{
    private readonly ApplicationDbContext _context;

    public NotificationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Notification>> GetUserNotificationsAsync(int? userId, bool unreadOnly = false)
    {
        var query = _context.Notifications.AsNoTracking().AsQueryable();

        if (userId.HasValue)
        {
            query = query.Where(n => n.UserId == null || n.UserId == userId.Value);
        }

        if (unreadOnly)
        {
            query = query.Where(n => !n.IsRead);
        }

        return await query.OrderByDescending(n => n.CreatedDate).Take(30).ToListAsync();
    }

    public async Task<int> GetUnreadCountAsync(int? userId)
    {
        return await _context.Notifications
            .Where(n => (userId == null || n.UserId == null || n.UserId == userId.Value) && !n.IsRead)
            .CountAsync();
    }

    public async Task MarkAsReadAsync(int notificationId)
    {
        var notification = await _context.Notifications.FindAsync(notificationId);
        if (notification != null)
        {
            notification.IsRead = true;
            await _context.SaveChangesAsync();
        }
    }

    public async Task CreateNotificationAsync(string message, NotificationType type, int? entityId = null, int? targetUserId = null)
    {
        var notification = new Notification
        {
            UserId = targetUserId,
            Message = message,
            NotificationType = type,
            EntityId = entityId,
            IsRead = false,
            CreatedDate = DateTime.UtcNow
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
    }
}
