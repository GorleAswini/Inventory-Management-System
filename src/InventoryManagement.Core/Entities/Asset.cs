using InventoryManagement.Core.Enums;

namespace InventoryManagement.Core.Entities;

public class Asset
{
    public int Id { get; set; }
    public string AssetCode { get; set; } = string.Empty;
    public string AssetName { get; set; } = string.Empty;
    
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
    
    public string? Description { get; set; }
    public string? SerialNumber { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public decimal? PurchaseCost { get; set; }
    public string? Location { get; set; }
    public string? Department { get; set; }

    public int? AssignedToUserId { get; set; }
    public User? AssignedToUser { get; set; }

    public AssetStatus Status { get; set; } = AssetStatus.Available;

    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
}
