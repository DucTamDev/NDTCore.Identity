namespace NDTCore.Identity.Domain.Constants;

/// <summary>
/// Standard claim type constants to avoid hard-coding claim type strings
/// </summary>
public static class ClaimTypes
{
    /// <summary>
    /// Permission claim type
    /// </summary>
    public const string Permission = "permission";

    /// <summary>
    /// Role claim type (standard)
    /// </summary>
    public const string Role = System.Security.Claims.ClaimTypes.Role;

    /// <summary>
    /// Name identifier claim type (standard)
    /// </summary>
    public const string NameIdentifier = System.Security.Claims.ClaimTypes.NameIdentifier;

    /// <summary>
    /// Subject claim type (JWT standard)
    /// </summary>
    public const string Subject = "sub";

    /// <summary>
    /// Unique name claim type (JWT standard)
    /// </summary>
    public const string UniqueName = "unique_name";

    /// <summary>
    /// JWT ID claim type (JWT standard)
    /// </summary>
    public const string JwtId = "jti";
}

