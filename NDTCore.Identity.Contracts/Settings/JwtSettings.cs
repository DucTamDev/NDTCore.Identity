namespace NDTCore.Identity.Contracts.Settings;

/// <summary>
/// JWT token configuration settings
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// Configuration section name
    /// </summary>
    public const string SectionName = "JwtSettings";

    /// <summary>
    /// Secret key for signing JWT tokens
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// Token issuer
    /// </summary>
    public string Issuer { get; set; } = "NDTCore.Identity";

    /// <summary>
    /// Token audience
    /// </summary>
    public string Audience { get; set; } = "NDTCore.Clients";

    /// <summary>
    /// Access token expiration time in minutes
    /// </summary>
    public int AccessTokenExpirationMinutes { get; set; } = 15;

    /// <summary>
    /// Refresh token expiration time in days
    /// </summary>
    public int RefreshTokenExpirationDays { get; set; } = 30;
}

