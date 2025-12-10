namespace NDTCore.Identity.Domain.Constants.Authorization.Policies;

/// <summary>
/// Domain constants for policy names used in authorization
/// </summary>
public static class PolicyNames
{
    /// <summary>
    /// Policy that requires any admin-level permission
    /// </summary>
    public const string AdminOnly = nameof(AdminOnly);

    /// <summary>
    /// Policy that requires any user management permission
    /// </summary>
    public const string UserManagement = nameof(UserManagement);

    /// <summary>
    /// Policy that requires any role management permission
    /// </summary>
    public const string RoleManagement = nameof(RoleManagement);

    /// <summary>
    /// Policy that requires any system administration permission
    /// </summary>
    public const string SystemAdministration = nameof(SystemAdministration);

    /// <summary>
    /// Policy that requires authentication permission management
    /// </summary>
    public const string AuthenticationManagement = nameof(AuthenticationManagement);

    /// <summary>
    /// Gets all defined policy names
    /// </summary>
    public static IReadOnlyList<string> GetAll() => new[]
    {
        AdminOnly,
        UserManagement,
        RoleManagement,
        SystemAdministration,
        AuthenticationManagement
    };
}

