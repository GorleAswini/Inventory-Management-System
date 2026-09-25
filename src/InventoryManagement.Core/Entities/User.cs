namespace InventoryManagement.Core.Entities;

public class User
{
    public int Id { get; set; }
    public string EmployeeId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? MobileNumber { get; set; }
    public string? Department { get; set; }
    
    public int RoleId { get; set; }
    public Role? Role { get; set; }
    
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedDate { get; set; }

    public ICollection<Asset> AssignedAssets { get; set; } = new List<Asset>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
