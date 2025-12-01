using System.ComponentModel.DataAnnotations;

namespace NDTCore.Identity.Contracts.DTOs.Auth;

public class ForgotPasswordRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;
}
