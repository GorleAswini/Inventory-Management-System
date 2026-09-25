using InventoryManagement.Core.DTOs;

namespace InventoryManagement.Core.Interfaces.Services;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetDashboardMetricsAsync(int? currentUserId = null);
}
