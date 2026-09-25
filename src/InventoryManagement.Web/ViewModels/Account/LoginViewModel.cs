using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Web.ViewModels.Account;

public class LoginViewModel
{
    [Required(ErrorMessage = "Username or Email is required")]
    [Display(Name = "Username or Email")]
    public string UsernameOrEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Remember Me")]
    public bool RememberMe { get; set; }
}
