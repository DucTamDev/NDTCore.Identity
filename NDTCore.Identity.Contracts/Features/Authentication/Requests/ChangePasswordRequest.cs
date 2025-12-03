using System.ComponentModel.DataAnnotations;

namespace NDTCore.Identity.Contracts.Features.Authentication.Requests;

/// <summary>
/// Request model for changing password
/// </summary>
public class ChangePasswordRequest
{
    /// <summary>
    /// Current password
    /// </summary>
    [Required(ErrorMessage = "Current password is required")]
    public string CurrentPassword { get; set; } = string.Empty;

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

