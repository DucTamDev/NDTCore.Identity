using NDTCore.Identity.Contracts.Features.Authentication.DTOs;

namespace NDTCore.Identity.Contracts.Features.Authentication.Responses;

/// <summary>
/// Authentication response containing tokens and user information
/// </summary>
public class AuthenticationResponse
{
    /// <summary>
    /// JWT access token
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Refresh token for obtaining new access tokens
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// Token expiration time in UTC
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// User information
    /// </summary>
    public UserInfoDto User { get; set; } = new();
}

