using System.ComponentModel.DataAnnotations;

namespace NDTCore.Identity.Contracts.Features.Authentication.Requests;

/// <summary>
/// Request model for forgot password
/// </summary>
public class ForgotPasswordRequest
{
    /// <summary>
    /// User email address
    /// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;
}

