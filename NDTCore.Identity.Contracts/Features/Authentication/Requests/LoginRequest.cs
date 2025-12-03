using System.ComponentModel.DataAnnotations;

namespace NDTCore.Identity.Contracts.Features.Authentication.Requests;

/// <summary>
/// Request model for user login
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// User email address
    /// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User password
    /// </summary>
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Remember me option
    /// </summary>
    public bool RememberMe { get; set; }
}

