using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryManagement.Web.ViewModels.Users;

public class UserFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Employee ID is mandatory.")]
    [Display(Name = "Employee ID")]
    public string EmployeeId { get; set; } = string.Empty;

    [Required(ErrorMessage = "First Name is mandatory.")]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last Name is mandatory.")]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Username is mandatory.")]
    [Display(Name = "Username")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is mandatory.")]
    [EmailAddress(ErrorMessage = "Please provide a valid email address.")]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Password")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [Display(Name = "Mobile Number")]
    public string? MobileNumber { get; set; }

    [Display(Name = "Department")]
    public string? Department { get; set; }

    [Required(ErrorMessage = "Role is mandatory.")]
    [Display(Name = "Role")]
    public int RoleId { get; set; }

    public IEnumerable<SelectListItem> RoleOptions { get; set; } = new List<SelectListItem>();
}
