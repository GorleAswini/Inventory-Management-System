using InventoryManagement.Core.Entities;
using InventoryManagement.Core.Enums;

namespace InventoryManagement.Core.Interfaces.Services;

public interface INotificationService
{
    Task<List<Notification>> GetUserNotificationsAsync(int? userId, bool unreadOnly = false);
    Task<int> GetUnreadCountAsync(int? userId);
    Task MarkAsReadAsync(int notificationId);
    Task CreateNotificationAsync(string message, NotificationType type, int? entityId = null, int? targetUserId = null);
}
