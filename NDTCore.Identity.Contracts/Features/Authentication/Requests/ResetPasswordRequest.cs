using System.ComponentModel.DataAnnotations;

namespace NDTCore.Identity.Contracts.Features.Authentication.Requests;

/// <summary>
/// Request model for resetting password
/// </summary>
public class ResetPasswordRequest
{
    /// <summary>
    /// User email address
    /// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Password reset token
    /// </summary>
    [Required(ErrorMessage = "Token is required")]
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// New password
    /// </summary>
    [Required(ErrorMessage = "New password is required")]
    [StringLength(100, MinimumLength = 6)]
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// Confirm new password
    /// </summary>
    [Required(ErrorMessage = "Confirm password is required")]
    [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

