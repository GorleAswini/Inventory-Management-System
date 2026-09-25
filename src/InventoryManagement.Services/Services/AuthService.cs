using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using InventoryManagement.Core.Entities;
using InventoryManagement.Core.Interfaces.Services;
using InventoryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Services.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;

    public AuthService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(bool Success, string Message, User? User)> ValidateUserAsync(string usernameOrEmail, string password)
    {
        if (string.IsNullOrWhiteSpace(usernameOrEmail) || string.IsNullOrWhiteSpace(password))
        {
            return (false, "Username and password are required.", null);
        }

        var normalizedInput = usernameOrEmail.Trim().ToLower();
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Username.ToLower() == normalizedInput || u.Email.ToLower() == normalizedInput);

        if (user == null)
        {
            return (false, "Invalid username or password.", null);
        }

        // Business Rule BR-007: Deactivated users cannot log into the system
        if (!user.IsActive)
        {
            return (false, "Your account has been deactivated. Please contact the Asset Manager.", null);
        }

        // For default demo accounts or hashed passwords
        if (!VerifyPassword(password, user.PasswordHash))
        {
            return (false, "Invalid username or password.", null);
        }

        // Record successful login in Audit Log
        _context.AuditLogs.Add(new AuditLog
        {
            Action = "Login",
            EntityName = "User",
            EntityId = user.Id,
            PerformedBy = user.Username,
            Description = $"User {user.Username} successfully logged into the system.",
            CreatedDate = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        return (true, "Login successful.", user);
    }

    public async Task<(bool Success, string Message)> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
        {
            return (false, "User not found.");
        }

        if (!VerifyPassword(currentPassword, user.PasswordHash))
        {
            return (false, "Current password is incorrect.");
        }

        // Suggested Password Rules (SRS Section 7.9): Min 8 chars, 1 uppercase, 1 lowercase, 1 number, 1 special char
        if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 8)
        {
            return (false, "New password must be at least 8 characters long.");
        }

        if (!Regex.IsMatch(newPassword, @"[A-Z]") ||
            !Regex.IsMatch(newPassword, @"[a-z]") ||
            !Regex.IsMatch(newPassword, @"[0-9]") ||
            !Regex.IsMatch(newPassword, @"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]"))
        {
            return (false, "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.");
        }

        user.PasswordHash = HashPassword(newPassword);
        user.UpdatedDate = DateTime.UtcNow;

        _context.AuditLogs.Add(new AuditLog
        {
            Action = "PasswordChange",
            EntityName = "User",
            EntityId = user.Id,
            PerformedBy = user.Username,
            Description = $"User {user.Username} updated their account password.",
            CreatedDate = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return (true, "Password changed successfully.");
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "IMS_SALT_2026"));
        return Convert.ToBase64String(hashedBytes);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        // For convenience with seeded demo password
        if (passwordHash == "AQAAAAIAAYagAAAAEG4zX2qV+demoHashExample==")
        {
            return password == "Admin@1234" || password == "Executive@1234" || password == "admin123" || password == "password";
        }

        return HashPassword(password) == passwordHash;
    }
}
