using InventoryManagement.Core.Entities;
using InventoryManagement.Core.Enums;
using InventoryManagement.Core.Interfaces.Services;
using InventoryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Services.Services;

public class AssetService : IAssetService
{
    private readonly ApplicationDbContext _context;
    private readonly INotificationService _notificationService;

    public AssetService(ApplicationDbContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<List<Asset>> GetAllAsync(string? search = null, int? categoryId = null, string? status = null, string? location = null)
    {
        var query = _context.Assets
            .Include(a => a.Category)
            .Include(a => a.AssignedToUser)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim().ToLower();
            query = query.Where(a =>
                a.AssetCode.ToLower().Contains(search) ||
                a.AssetName.ToLower().Contains(search) ||
                (a.SerialNumber != null && a.SerialNumber.ToLower().Contains(search)) ||
                (a.Location != null && a.Location.ToLower().Contains(search)));
        }

        if (categoryId.HasValue && categoryId.Value > 0)
        {
            query = query.Where(a => a.CategoryId == categoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<AssetStatus>(status, true, out var parsedStatus))
        {
            query = query.Where(a => a.Status == parsedStatus);
        }

        if (!string.IsNullOrWhiteSpace(location))
        {
            query = query.Where(a => a.Location == location);
        }

        return await query.OrderByDescending(a => a.CreatedDate).ToListAsync();
    }

    public async Task<Asset?> GetByIdAsync(int id)
    {
        return await _context.Assets
            .Include(a => a.Category)
            .Include(a => a.AssignedToUser)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<bool> AssetCodeExistsAsync(string assetCode, int? excludeId = null)
    {
        return await _context.Assets.AnyAsync(a =>
            a.AssetCode.ToLower() == assetCode.Trim().ToLower() &&
            (!excludeId.HasValue || a.Id != excludeId.Value));
    }

    public async Task<(bool Success, string Message)> CreateAssetAsync(Asset asset, string user)
    {
        // Business Rule BR-001: Unique Asset Code
        if (await AssetCodeExistsAsync(asset.AssetCode))
        {
            return (false, $"Asset Code '{asset.AssetCode}' already exists. Please choose a unique Asset Code.");
        }

        // Business Rule BR-008: Cannot assign to inactive user
        if (asset.AssignedToUserId.HasValue)
        {
            var assignedUser = await _context.Users.FindAsync(asset.AssignedToUserId.Value);
            if (assignedUser == null || !assignedUser.IsActive)
            {
                return (false, "An asset cannot be assigned to an inactive or non-existent user.");
            }
        }

        asset.CreatedBy = user;
        asset.CreatedDate = DateTime.UtcNow;

        _context.Assets.Add(asset);
        await _context.SaveChangesAsync();

        // Business Rule BR-012: Audit Log
        _context.AuditLogs.Add(new AuditLog
        {
            Action = "Created",
            EntityName = "Asset",
            EntityId = asset.Id,
            PerformedBy = user,
            Description = $"Created new asset {asset.AssetName} ({asset.AssetCode}) in category ID {asset.CategoryId} with status {asset.Status}.",
            CreatedDate = DateTime.UtcNow
        });

        // Business Rule BR-011: Notification
        await _notificationService.CreateNotificationAsync(
            $"New Inventory Added: {asset.AssetName} - {asset.AssetCode} has been added by {user}.",
            NotificationType.AssetCreated,
            asset.Id);

        await _context.SaveChangesAsync();
        return (true, "Asset created successfully.");
    }

    public async Task<(bool Success, string Message)> UpdateAssetAsync(Asset asset, string user)
    {
        var existing = await _context.Assets.FindAsync(asset.Id);
        if (existing == null)
        {
            return (false, "Asset not found.");
        }

        // Business Rule BR-001: Unique Asset Code
        if (await AssetCodeExistsAsync(asset.AssetCode, asset.Id))
        {
            return (false, $"Asset Code '{asset.AssetCode}' already exists on another asset.");
        }

        // Business Rule BR-008: Inactive check
        if (asset.AssignedToUserId.HasValue)
        {
            var assignedUser = await _context.Users.FindAsync(asset.AssignedToUserId.Value);
            if (assignedUser == null || !assignedUser.IsActive)
            {
                return (false, "An asset cannot be assigned to an inactive or non-existent user.");
            }
        }

        existing.AssetCode = asset.AssetCode;
        existing.AssetName = asset.AssetName;
        existing.CategoryId = asset.CategoryId;
        existing.Description = asset.Description;
        existing.SerialNumber = asset.SerialNumber;
        existing.PurchaseDate = asset.PurchaseDate;
        existing.PurchaseCost = asset.PurchaseCost;
        existing.Location = asset.Location;
        existing.Department = asset.Department;
        existing.AssignedToUserId = asset.AssignedToUserId;
        existing.Status = asset.Status;
        existing.UpdatedBy = user;
        existing.UpdatedDate = DateTime.UtcNow;

        // Business Rule BR-012: Audit Log
        _context.AuditLogs.Add(new AuditLog
        {
            Action = "Updated",
            EntityName = "Asset",
            EntityId = existing.Id,
            PerformedBy = user,
            Description = $"Updated asset {existing.AssetName} ({existing.AssetCode}). Status: {existing.Status}.",
            CreatedDate = DateTime.UtcNow
        });

        // Business Rule BR-011: Notification
        await _notificationService.CreateNotificationAsync(
            $"Inventory Updated: {existing.AssetName} ({existing.AssetCode}) modified by {user}.",
            NotificationType.AssetUpdated,
            existing.Id);

        await _context.SaveChangesAsync();
        return (true, "Asset updated successfully.");
    }

    public async Task<(bool Success, string Message)> DeleteAssetAsync(int id, string user)
    {
        var asset = await _context.Assets.FindAsync(id);
        if (asset == null)
        {
            return (false, "Asset not found.");
        }

        var assetInfo = $"{asset.AssetName} ({asset.AssetCode})";

        _context.Assets.Remove(asset);

        // Business Rule BR-012: Audit Log
        _context.AuditLogs.Add(new AuditLog
        {
            Action = "Deleted",
            EntityName = "Asset",
            EntityId = id,
            PerformedBy = user,
            Description = $"Deleted asset {assetInfo}.",
            CreatedDate = DateTime.UtcNow
        });

        // Business Rule BR-011: Notification
        await _notificationService.CreateNotificationAsync(
            $"Inventory Deleted: {assetInfo} was deleted by {user}.",
            NotificationType.AssetDeleted,
            id);

        await _context.SaveChangesAsync();
        return (true, "Asset deleted successfully.");
    }
}
