namespace NDTCore.Identity.Contracts.Features.Permissions.DTOs;

/// <summary>
/// User permissions data transfer object
/// </summary>
public class UserPermissionsDto
{
    /// <summary>
    /// User ID
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// User name
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// List of user's roles
    /// </summary>
    public List<string> Roles { get; set; } = new();

    /// <summary>
    /// List of permissions assigned directly to user (via UserClaims)
    /// </summary>
    public List<string> DirectPermissions { get; set; } = new();

    /// <summary>
    /// List of permissions inherited from roles (via RoleClaims)
    /// </summary>
    public List<string> RolePermissions { get; set; } = new();

    /// <summary>
    /// All effective permissions (direct + role permissions, deduplicated)
    /// </summary>
    public List<string> EffectivePermissions { get; set; } = new();
}

