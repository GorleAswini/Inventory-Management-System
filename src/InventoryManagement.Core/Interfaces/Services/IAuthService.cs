using InventoryManagement.Core.Entities;

namespace InventoryManagement.Core.Interfaces.Services;

public interface IAuthService
{
    Task<(bool Success, string Message, User? User)> ValidateUserAsync(string usernameOrEmail, string password);
    Task<(bool Success, string Message)> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    Task<User?> GetUserByIdAsync(int userId);
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}
