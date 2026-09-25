using InventoryManagement.Core.Entities;

namespace InventoryManagement.Core.Interfaces.Services;

public interface IUserService
{
    Task<List<User>> GetAllUsersAsync(string? search = null);
    Task<User?> GetUserByIdAsync(int id);
    Task<bool> UsernameExistsAsync(string username, int? excludeId = null);
    Task<bool> EmailExistsAsync(string email, int? excludeId = null);
    Task<(bool Success, string Message)> CreateUserAsync(User user, string initialPassword);
    Task<(bool Success, string Message)> UpdateUserAsync(User user);
    Task<(bool Success, string Message)> ToggleStatusAsync(int id);
}
