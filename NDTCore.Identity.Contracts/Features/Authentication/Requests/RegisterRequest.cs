using System.ComponentModel.DataAnnotations;

namespace NDTCore.Identity.Contracts.Features.Authentication.Requests;

/// <summary>
/// Request model for user registration
/// </summary>
public class RegisterRequest
{
    /// <summary>
    /// User email address
    /// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Username
    /// </summary>
    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, MinimumLength = 3)]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Password
    /// </summary>
    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Password confirmation
    /// </summary>
    [Required(ErrorMessage = "Confirm password is required")]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;

    /// <summary>
    /// First name
    /// </summary>
    [Required(ErrorMessage = "First name is required")]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Last name
    /// </summary>
    [Required(ErrorMessage = "Last name is required")]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Phone number (optional)
    /// </summary>
    [Phone(ErrorMessage = "Invalid phone number")]
    public string? PhoneNumber { get; set; }
}

