using InventoryManagement.Core.Constants;
using InventoryManagement.Core.Entities;
using InventoryManagement.Core.Interfaces.Services;
using InventoryManagement.Infrastructure.Data;
using InventoryManagement.Web.ViewModels.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Web.Controllers;

[Authorize(Roles = Roles.AssetManager)]
public class UsersController : Controller
{
    private readonly IUserService _userService;
    private readonly ApplicationDbContext _context;

    public UsersController(IUserService userService, ApplicationDbContext context)
    {
        _userService = userService;
        _context = context;
    }

    public async Task<IActionResult> Index(string? search)
    {
        ViewBag.Search = search;
        var users = await _userService.GetAllUsersAsync(search);
        return View(users);
    }

    public async Task<IActionResult> Create()
    {
        var model = new UserFormViewModel
        {
            RoleOptions = await GetRolesSelectListAsync()
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserFormViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Password))
        {
            ModelState.AddModelError("Password", "Password is required for new users.");
        }

        if (!ModelState.IsValid)
        {
            model.RoleOptions = await GetRolesSelectListAsync();
            return View(model);
        }

        var user = new User
        {
            EmployeeId = model.EmployeeId.Trim(),
            FirstName = model.FirstName.Trim(),
            LastName = model.LastName.Trim(),
            Username = model.Username.Trim(),
            Email = model.Email.Trim(),
            MobileNumber = model.MobileNumber,
            Department = model.Department,
            RoleId = model.RoleId,
            IsActive = true
        };

        var (success, message) = await _userService.CreateUserAsync(user, model.Password!);
        if (!success)
        {
            ModelState.AddModelError(string.Empty, message);
            model.RoleOptions = await GetRolesSelectListAsync();
            return View(model);
        }

        TempData["SuccessMessage"] = message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var (success, message) = await _userService.ToggleStatusAsync(id);
        if (success)
        {
            TempData["SuccessMessage"] = message;
        }
        else
        {
            TempData["ErrorMessage"] = message;
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task<IEnumerable<SelectListItem>> GetRolesSelectListAsync()
    {
        var roles = await _context.Roles.OrderBy(r => r.Name).ToListAsync();
        return roles.Select(r => new SelectListItem
        {
            Value = r.Id.ToString(),
            Text = r.Name
        });
    }
}
