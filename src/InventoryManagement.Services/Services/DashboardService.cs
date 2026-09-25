using InventoryManagement.Core.DTOs;
using InventoryManagement.Core.Enums;
using InventoryManagement.Core.Interfaces.Services;
using InventoryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Services.Services;

public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _context;
    private readonly INotificationService _notificationService;

    public DashboardService(ApplicationDbContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<DashboardSummaryDto> GetDashboardMetricsAsync(int? currentUserId = null)
    {
        var assets = await _context.Assets
            .Include(a => a.Category)
            .AsNoTracking()
            .ToListAsync();

        var categoriesCount = await _context.Categories.Where(c => c.IsActive).CountAsync();
        var unreadNotifications = await _notificationService.GetUnreadCountAsync(currentUserId);

        var recentActivities = await _context.AuditLogs
            .OrderByDescending(a => a.CreatedDate)
            .Take(5)
            .AsNoTracking()
            .ToListAsync();

        var recentNotifications = await _notificationService.GetUserNotificationsAsync(currentUserId);

        var summary = new DashboardSummaryDto
        {
            TotalAssets = assets.Count,
            ActiveAssets = assets.Count(a => a.Status == AssetStatus.Available || a.Status == AssetStatus.Assigned),
            AvailableAssets = assets.Count(a => a.Status == AssetStatus.Available),
            AssignedAssets = assets.Count(a => a.Status == AssetStatus.Assigned),
            UnderMaintenance = assets.Count(a => a.Status == AssetStatus.UnderMaintenance),
            RetiredAssets = assets.Count(a => a.Status == AssetStatus.Retired),
            NumberOfCategories = categoriesCount,
            UnreadNotifications = unreadNotifications,
            RecentActivities = recentActivities,
            RecentNotifications = recentNotifications.Take(5).ToList()
        };

        // Assets by Category
        summary.AssetsByCategory = assets
            .GroupBy(a => a.Category?.Name ?? "Uncategorized")
            .ToDictionary(g => g.Key, g => g.Count());

        // Assets by Status
        summary.AssetsByStatus = assets
            .GroupBy(a => a.Status.ToString())
            .ToDictionary(g => g.Key, g => g.Count());

        // Assets by Location
        summary.AssetsByLocation = assets
            .Where(a => !string.IsNullOrEmpty(a.Location))
            .GroupBy(a => a.Location!)
            .ToDictionary(g => g.Key, g => g.Count());

        // Assets by Department
        summary.AssetsByDepartment = assets
            .Where(a => !string.IsNullOrEmpty(a.Department))
            .GroupBy(a => a.Department!)
            .ToDictionary(g => g.Key, g => g.Count());

        return summary;
    }
}
