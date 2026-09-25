using InventoryManagement.Core.Entities;
using InventoryManagement.Core.Interfaces.Services;
using InventoryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Services.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly IAuthService _authService;

    public UserService(ApplicationDbContext context, IAuthService authService)
    {
        _context = context;
        _authService = authService;
    }

    public async Task<List<User>> GetAllUsersAsync(string? search = null)
    {
        var query = _context.Users
            .Include(u => u.Role)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim().ToLower();
            query = query.Where(u =>
                u.FirstName.ToLower().Contains(search) ||
                u.LastName.ToLower().Contains(search) ||
                u.Username.ToLower().Contains(search) ||
                u.Email.ToLower().Contains(search) ||
                (u.Department != null && u.Department.ToLower().Contains(search)));
        }

        return await query.OrderBy(u => u.FirstName).ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<bool> UsernameExistsAsync(string username, int? excludeId = null)
    {
        return await _context.Users.AnyAsync(u =>
            u.Username.ToLower() == username.Trim().ToLower() &&
            (!excludeId.HasValue || u.Id != excludeId.Value));
    }

    public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
    {
        return await _context.Users.AnyAsync(u =>
            u.Email.ToLower() == email.Trim().ToLower() &&
            (!excludeId.HasValue || u.Id != excludeId.Value));
    }

    public async Task<(bool Success, string Message)> CreateUserAsync(User user, string initialPassword)
    {
        if (await UsernameExistsAsync(user.Username))
        {
            return (false, "Username is already taken.");
        }

        if (await EmailExistsAsync(user.Email))
        {
            return (false, "Email address is already in use.");
        }

        user.PasswordHash = _authService.HashPassword(initialPassword);
        user.CreatedDate = DateTime.UtcNow;
        user.IsActive = true;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _context.AuditLogs.Add(new AuditLog
        {
            Action = "Created",
            EntityName = "User",
            EntityId = user.Id,
            PerformedBy = "System/Manager",
            Description = $"User {user.Username} ({user.Email}) was created.",
            CreatedDate = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        return (true, "User created successfully.");
    }

    public async Task<(bool Success, string Message)> UpdateUserAsync(User user)
    {
        var existing = await _context.Users.FindAsync(user.Id);
        if (existing == null)
        {
            return (false, "User not found.");
        }

        if (await EmailExistsAsync(user.Email, user.Id))
        {
            return (false, "Email address is already in use by another user.");
        }

        existing.FirstName = user.FirstName;
        existing.LastName = user.LastName;
        existing.Email = user.Email;
        existing.MobileNumber = user.MobileNumber;
        existing.Department = user.Department;
        existing.RoleId = user.RoleId;
        existing.UpdatedDate = DateTime.UtcNow;

        _context.AuditLogs.Add(new AuditLog
        {
            Action = "Updated",
            EntityName = "User",
            EntityId = existing.Id,
            PerformedBy = "System/Manager",
            Description = $"User {existing.Username} profile was modified.",
            CreatedDate = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return (true, "User updated successfully.");
    }

    public async Task<(bool Success, string Message)> ToggleStatusAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return (false, "User not found.");
        }

        user.IsActive = !user.IsActive;
        user.UpdatedDate = DateTime.UtcNow;

        var statusText = user.IsActive ? "Activated" : "Deactivated";

        _context.AuditLogs.Add(new AuditLog
        {
            Action = statusText,
            EntityName = "User",
            EntityId = user.Id,
            PerformedBy = "System/Manager",
            Description = $"User {user.Username} was {statusText.ToLower()}.",
            CreatedDate = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return (true, $"User has been {statusText.ToLower()} successfully.");
    }
}
