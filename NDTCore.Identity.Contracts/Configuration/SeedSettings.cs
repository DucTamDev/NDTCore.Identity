namespace NDTCore.Identity.Contracts.Configuration;

/// <summary>
/// Configuration settings for data seeding
/// </summary>
public class SeedSettings
{
    /// <summary>
    /// Configuration section name
    /// </summary>
    public const string SectionName = "SeedSettings";

    /// <summary>
    /// Enable data seeding on application startup
    /// </summary>
    public bool EnableSeeding { get; set; } = true;

    /// <summary>
    /// Admin user seed configuration
    /// </summary>
    public AdminUserSeedSettings AdminUser { get; set; } = new();
}

/// <summary>
/// Admin user seed configuration
/// </summary>
public class AdminUserSeedSettings
{
    /// <summary>
    /// Admin user email
    /// </summary>
    public string Email { get; set; } = "admin@ndtcore.com";

    /// <summary>
    /// Admin username
    /// </summary>
    public string UserName { get; set; } = "admin";

    /// <summary>
    /// Admin password (should be changed after first login)
    /// </summary>
    public string Password { get; set; } = "Admin@123456";

    /// <summary>
    /// Admin first name
    /// </summary>
    public string FirstName { get; set; } = "System";

    /// <summary>
    /// Admin last name
    /// </summary>
    public string LastName { get; set; } = "Administrator";
}

