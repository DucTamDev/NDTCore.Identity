namespace NDTCore.Identity.Contracts.Settings;

/// <summary>
/// Configuration settings for refresh token validation
/// </summary>
public class TokenValidationSettings
{
    /// <summary>
    /// Configuration section name
    /// </summary>
    public const string SectionName = "TokenValidationSettings";

    /// <summary>
    /// Enable IP address validation for refresh tokens
    /// </summary>
    public bool ValidateIpAddress { get; set; } = false;

    /// <summary>
    /// Enable device fingerprint validation for refresh tokens
    /// </summary>
    public bool ValidateDeviceFingerprint { get; set; } = false;

    /// <summary>
    /// Maximum number of active refresh tokens allowed per user
    /// </summary>
    public int MaxActiveTokensPerUser { get; set; } = 5;

    /// <summary>
    /// Grace period in minutes for token rotation (0 = immediate revocation)
    /// </summary>
    public int TokenRotationGracePeriodMinutes { get; set; } = 0;

    /// <summary>
    /// Automatically revoke old refresh tokens after N days of inactivity
    /// </summary>
    public bool AutoRevokeInactiveTokens { get; set; } = true;

    /// <summary>
    /// Number of days before inactive tokens are auto-revoked
    /// </summary>
    public int InactiveTokenRevocationDays { get; set; } = 90;

    /// <summary>
    /// Enable token reuse detection (security feature)
    /// </summary>
    public bool DetectTokenReuse { get; set; } = true;

    /// <summary>
    /// Action to take on token reuse detection: "RevokeAll", "RevokeChain", "LogOnly"
    /// </summary>
    public string TokenReuseAction { get; set; } = "RevokeAll";
}

