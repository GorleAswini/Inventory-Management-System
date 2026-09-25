using InventoryManagement.Core.Entities;

namespace InventoryManagement.Core.Interfaces.Services;

public interface IReportService
{
    Task<List<Asset>> GetInventoryRegisterAsync(string? category = null, string? status = null, string? location = null, string? department = null);
    Task<Dictionary<string, List<Asset>>> GetAssetStatusReportAsync();
    Task<Dictionary<string, List<Asset>>> GetCategoryReportAsync();
    Task<Dictionary<string, List<Asset>>> GetLocationReportAsync();
    Task<Dictionary<string, List<Asset>>> GetDepartmentReportAsync();
    Task<List<AuditLog>> GetActivityReportAsync(DateTime? fromDate = null, DateTime? toDate = null);
}
