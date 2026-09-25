using InventoryManagement.Core.Entities;

namespace InventoryManagement.Core.DTOs;

public class DashboardSummaryDto
{
    public int TotalAssets { get; set; }
    public int ActiveAssets { get; set; }
    public int AvailableAssets { get; set; }
    public int AssignedAssets { get; set; }
    public int UnderMaintenance { get; set; }
    public int RetiredAssets { get; set; }
    public int NumberOfCategories { get; set; }
    public int UnreadNotifications { get; set; }

    public Dictionary<string, int> AssetsByCategory { get; set; } = new();
    public Dictionary<string, int> AssetsByStatus { get; set; } = new();
    public Dictionary<string, int> AssetsByLocation { get; set; } = new();
    public Dictionary<string, int> AssetsByDepartment { get; set; } = new();

    public List<AuditLog> RecentActivities { get; set; } = new();
    public List<Notification> RecentNotifications { get; set; } = new();
}
