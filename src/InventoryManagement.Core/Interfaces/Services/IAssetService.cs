using InventoryManagement.Core.Entities;

namespace InventoryManagement.Core.Interfaces.Services;

public interface IAssetService
{
    Task<List<Asset>> GetAllAsync(string? search = null, int? categoryId = null, string? status = null, string? location = null);
    Task<Asset?> GetByIdAsync(int id);
    Task<bool> AssetCodeExistsAsync(string assetCode, int? excludeId = null);
    Task<(bool Success, string Message)> CreateAssetAsync(Asset asset, string user);
    Task<(bool Success, string Message)> UpdateAssetAsync(Asset asset, string user);
    Task<(bool Success, string Message)> DeleteAssetAsync(int id, string user);
}
