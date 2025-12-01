using System.ComponentModel.DataAnnotations;

namespace NDTCore.Identity.Contracts.DTOs.Auth;

public class RefreshTokenRequest
{
    [Required]
    public string AccessToken { get; set; } = string.Empty;

    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
