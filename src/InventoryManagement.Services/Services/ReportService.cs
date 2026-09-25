using InventoryManagement.Core.Entities;
using InventoryManagement.Core.Interfaces.Services;
using InventoryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Services.Services;

public class ReportService : IReportService
{
    private readonly ApplicationDbContext _context;

    public ReportService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Asset>> GetInventoryRegisterAsync(string? category = null, string? status = null, string? location = null, string? department = null)
    {
        var query = _context.Assets
            .Include(a => a.Category)
            .Include(a => a.AssignedToUser)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(a => a.Category != null && a.Category.Name == category);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(a => a.Status.ToString() == status);
        }

        if (!string.IsNullOrWhiteSpace(location))
        {
            query = query.Where(a => a.Location == location);
        }

        if (!string.IsNullOrWhiteSpace(department))
        {
            query = query.Where(a => a.Department == department);
        }

        return await query.OrderBy(a => a.AssetCode).ToListAsync();
    }

    public async Task<Dictionary<string, List<Asset>>> GetAssetStatusReportAsync()
    {
        var assets = await _context.Assets
            .Include(a => a.Category)
            .Include(a => a.AssignedToUser)
            .AsNoTracking()
            .ToListAsync();

        return assets
            .GroupBy(a => a.Status.ToString())
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    public async Task<Dictionary<string, List<Asset>>> GetCategoryReportAsync()
    {
        var assets = await _context.Assets
            .Include(a => a.Category)
            .Include(a => a.AssignedToUser)
            .AsNoTracking()
            .ToListAsync();

        return assets
            .GroupBy(a => a.Category?.Name ?? "Uncategorized")
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    public async Task<Dictionary<string, List<Asset>>> GetLocationReportAsync()
    {
        var assets = await _context.Assets
            .Include(a => a.Category)
            .Include(a => a.AssignedToUser)
            .AsNoTracking()
            .ToListAsync();

        return assets
            .GroupBy(a => string.IsNullOrWhiteSpace(a.Location) ? "Unassigned Location" : a.Location)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    public async Task<Dictionary<string, List<Asset>>> GetDepartmentReportAsync()
    {
        var assets = await _context.Assets
            .Include(a => a.Category)
            .Include(a => a.AssignedToUser)
            .AsNoTracking()
            .ToListAsync();

        return assets
            .GroupBy(a => string.IsNullOrWhiteSpace(a.Department) ? "Unassigned Department" : a.Department)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    public async Task<List<AuditLog>> GetActivityReportAsync(DateTime? fromDate = null, DateTime? toDate = null)
    {
        var query = _context.AuditLogs.AsNoTracking().AsQueryable();

        if (fromDate.HasValue)
        {
            query = query.Where(a => a.CreatedDate >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(a => a.CreatedDate <= toDate.Value.AddDays(1));
        }

        return await query.OrderByDescending(a => a.CreatedDate).Take(100).ToListAsync();
    }
}
