using System.Security.Claims;
using InventoryManagement.Core.Entities;
using InventoryManagement.Core.Interfaces.Services;
using InventoryManagement.Infrastructure.Data;
using InventoryManagement.Web.ViewModels.Assets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Web.Controllers;

[Authorize]
public class AssetsController : Controller
{
    private readonly IAssetService _assetService;
    private readonly ApplicationDbContext _context;

    public AssetsController(IAssetService assetService, ApplicationDbContext context)
    {
        _assetService = assetService;
        _context = context;
    }

    public async Task<IActionResult> Index(string? search, int? categoryId, string? status, string? location)
    {
        var assets = await _assetService.GetAllAsync(search, categoryId, status, location);
        
        ViewBag.Search = search;
        ViewBag.CategoryId = categoryId;
        ViewBag.Status = status;
        ViewBag.Location = location;
        ViewBag.Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();

        return View(assets);
    }

    public async Task<IActionResult> Details(int id)
    {
        var asset = await _assetService.GetByIdAsync(id);
        if (asset == null)
        {
            return NotFound();
        }

        return View(asset);
    }

    public async Task<IActionResult> Create()
    {
        var model = new AssetFormViewModel
        {
            CategoryOptions = await GetCategorySelectListAsync(),
            UserOptions = await GetUserSelectListAsync()
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AssetFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.CategoryOptions = await GetCategorySelectListAsync();
            model.UserOptions = await GetUserSelectListAsync();
            return View(model);
        }

        var asset = new Asset
        {
            AssetCode = model.AssetCode.Trim().ToUpper(),
            AssetName = model.AssetName.Trim(),
            CategoryId = model.CategoryId,
            Description = model.Description,
            SerialNumber = model.SerialNumber,
            PurchaseDate = model.PurchaseDate,
            PurchaseCost = model.PurchaseCost,
            Location = model.Location,
            Department = model.Department,
            AssignedToUserId = model.AssignedToUserId,
            Status = model.Status
        };

        var userName = User.FindFirstValue("Username") ?? User.Identity?.Name ?? "User";
        var (success, message) = await _assetService.CreateAssetAsync(asset, userName);

        if (!success)
        {
            ModelState.AddModelError(string.Empty, message);
            model.CategoryOptions = await GetCategorySelectListAsync();
            model.UserOptions = await GetUserSelectListAsync();
            return View(model);
        }

        TempData["SuccessMessage"] = message;
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var asset = await _assetService.GetByIdAsync(id);
        if (asset == null)
        {
            return NotFound();
        }

        var model = new AssetFormViewModel
        {
            Id = asset.Id,
            AssetCode = asset.AssetCode,
            AssetName = asset.AssetName,
            CategoryId = asset.CategoryId,
            Description = asset.Description,
            SerialNumber = asset.SerialNumber,
            PurchaseDate = asset.PurchaseDate,
            PurchaseCost = asset.PurchaseCost,
            Location = asset.Location,
            Department = asset.Department,
            AssignedToUserId = asset.AssignedToUserId,
            Status = asset.Status,
            CategoryOptions = await GetCategorySelectListAsync(),
            UserOptions = await GetUserSelectListAsync()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AssetFormViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            model.CategoryOptions = await GetCategorySelectListAsync();
            model.UserOptions = await GetUserSelectListAsync();
            return View(model);
        }

        var asset = new Asset
        {
            Id = model.Id,
            AssetCode = model.AssetCode.Trim().ToUpper(),
            AssetName = model.AssetName.Trim(),
            CategoryId = model.CategoryId,
            Description = model.Description,
            SerialNumber = model.SerialNumber,
            PurchaseDate = model.PurchaseDate,
            PurchaseCost = model.PurchaseCost,
            Location = model.Location,
            Department = model.Department,
            AssignedToUserId = model.AssignedToUserId,
            Status = model.Status
        };

        var userName = User.FindFirstValue("Username") ?? User.Identity?.Name ?? "User";
        var (success, message) = await _assetService.UpdateAssetAsync(asset, userName);

        if (!success)
        {
            ModelState.AddModelError(string.Empty, message);
            model.CategoryOptions = await GetCategorySelectListAsync();
            model.UserOptions = await GetUserSelectListAsync();
            return View(model);
        }

        TempData["SuccessMessage"] = message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var userName = User.FindFirstValue("Username") ?? User.Identity?.Name ?? "User";
        var (success, message) = await _assetService.DeleteAssetAsync(id, userName);

        if (!success)
        {
            TempData["ErrorMessage"] = message;
        }
        else
        {
            TempData["SuccessMessage"] = message;
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task<IEnumerable<SelectListItem>> GetCategorySelectListAsync()
    {
        var categories = await _context.Categories
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();

        return categories.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Name
        });
    }

    private async Task<IEnumerable<SelectListItem>> GetUserSelectListAsync()
    {
        var users = await _context.Users
            .Where(u => u.IsActive)
            .OrderBy(u => u.FirstName)
            .ToListAsync();

        var list = users.Select(u => new SelectListItem
        {
            Value = u.Id.ToString(),
            Text = $"{u.FirstName} {u.LastName} ({u.Department ?? "General"})"
        }).ToList();

        list.Insert(0, new SelectListItem { Value = "", Text = "-- None (Available in Storage) --" });
        return list;
    }
}
