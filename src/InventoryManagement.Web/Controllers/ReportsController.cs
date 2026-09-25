using InventoryManagement.Core.Interfaces.Services;
using InventoryManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Web.Controllers;

[Authorize]
public class ReportsController : Controller
{
    private readonly IReportService _reportService;
    private readonly ApplicationDbContext _context;

    public ReportsController(IReportService reportService, ApplicationDbContext context)
    {
        _reportService = reportService;
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> InventoryRegister(string? category, string? status, string? location, string? department)
    {
        var assets = await _reportService.GetInventoryRegisterAsync(category, status, location, department);
        
        ViewBag.Category = category;
        ViewBag.Status = status;
        ViewBag.Location = location;
        ViewBag.Department = department;

        ViewBag.Categories = await _context.Categories.Where(c => c.IsActive).OrderBy(c => c.Name).ToListAsync();
        ViewBag.Locations = await _context.Assets.Where(a => a.Location != null).Select(a => a.Location!).Distinct().ToListAsync();
        ViewBag.Departments = await _context.Assets.Where(a => a.Department != null).Select(a => a.Department!).Distinct().ToListAsync();

        return View(assets);
    }

    public async Task<IActionResult> StatusReport()
    {
        var groupedAssets = await _reportService.GetAssetStatusReportAsync();
        return View(groupedAssets);
    }

    public async Task<IActionResult> CategoryReport()
    {
        var groupedAssets = await _reportService.GetCategoryReportAsync();
        return View(groupedAssets);
    }

    public async Task<IActionResult> LocationReport()
    {
        var groupedAssets = await _reportService.GetLocationReportAsync();
        return View(groupedAssets);
    }

    public async Task<IActionResult> DepartmentReport()
    {
        var groupedAssets = await _reportService.GetDepartmentReportAsync();
        return View(groupedAssets);
    }

    public async Task<IActionResult> ActivityReport(DateTime? fromDate, DateTime? toDate)
    {
        var logs = await _reportService.GetActivityReportAsync(fromDate, toDate);
        ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
        ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");
        return View(logs);
    }
}
