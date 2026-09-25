using System.ComponentModel.DataAnnotations;
using InventoryManagement.Core.Entities;
using InventoryManagement.Core.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryManagement.Web.ViewModels.Assets;

public class AssetFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Asset Code is mandatory.")]
    [Display(Name = "Asset Code")]
    [StringLength(50)]
    public string AssetCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Asset Name is mandatory.")]
    [Display(Name = "Asset Name")]
    [StringLength(150)]
    public string AssetName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Category is mandatory.")]
    [Display(Name = "Category")]
    public int CategoryId { get; set; }

    [Display(Name = "Description")]
    [StringLength(500)]
    public string? Description { get; set; }

    [Display(Name = "Serial Number")]
    [StringLength(100)]
    public string? SerialNumber { get; set; }

    [Display(Name = "Purchase Date")]
    [DataType(DataType.Date)]
    public DateTime? PurchaseDate { get; set; }

    [Display(Name = "Purchase Cost ($)")]
    [Range(0, 1000000, ErrorMessage = "Purchase cost must be a positive number.")]
    public decimal? PurchaseCost { get; set; }

    [Display(Name = "Location")]
    [StringLength(100)]
    public string? Location { get; set; }

    [Display(Name = "Department")]
    [StringLength(100)]
    public string? Department { get; set; }

    [Display(Name = "Assign To User")]
    public int? AssignedToUserId { get; set; }

    [Required(ErrorMessage = "Status is mandatory.")]
    [Display(Name = "Status")]
    public AssetStatus Status { get; set; } = AssetStatus.Available;

    public IEnumerable<SelectListItem> CategoryOptions { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> UserOptions { get; set; } = new List<SelectListItem>();
}
