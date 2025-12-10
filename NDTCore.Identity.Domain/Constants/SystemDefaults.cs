namespace NDTCore.Identity.Domain.Constants;

/// <summary>
/// System default values
/// </summary>
public static class SystemDefaults
{
    // User defaults
    public const bool DefaultUserIsActive = true;
    public const bool DefaultUserIsDeleted = false;

    // Token defaults
    public const int DefaultAccessTokenExpirationMinutes = 15;
    public const int DefaultRefreshTokenExpirationDays = 7;
    public const int DefaultMaxActiveTokensPerUser = 5;

    // Pagination defaults
    public const int DefaultPageNumber = 1;
    public const int DefaultPageSize = 10;
    public const int MaxPageSize = 100;

    // Password requirements
    public const int MinPasswordLength = 6;
    public const bool RequireDigit = true;
    public const bool RequireLowercase = true;
    public const bool RequireUppercase = true;
    public const bool RequireNonAlphanumeric = true;
}

