using System.ComponentModel.DataAnnotations;

namespace NDTCore.Identity.Contracts.Features.Authentication.Requests;

/// <summary>
/// Request model for refreshing access token
/// </summary>
public class RefreshTokenRequest
{
    /// <summary>
    /// Expired access token
    /// </summary>
    [Required(ErrorMessage = "Access token is required")]
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Refresh token
    /// </summary>
    [Required(ErrorMessage = "Refresh token is required")]
    public string RefreshToken { get; set; } = string.Empty;
}

