namespace NDTCore.Identity.Contracts.Authorization.Permissions;

/// <summary>
/// Represents a permission definition with metadata
/// </summary>
public interface IPermissionDefinition
{
    /// <summary>
    /// Unique permission name/identifier (e.g., "Permissions.Users.View")
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Display name for UI (e.g., "View Users")
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    /// Module/category this permission belongs to (e.g., "Users", "Roles")
    /// </summary>
    string Module { get; }

    /// <summary>
    /// Optional description of what this permission allows
    /// </summary>
    string? Description { get; }

    /// <summary>
    /// Sort order for UI display
    /// </summary>
    int SortOrder { get; }

    /// <summary>
    /// Whether this is a system permission (cannot be deleted)
    /// </summary>
    bool IsSystemPermission { get; }

    /// <summary>
    /// Permission group within module (e.g., "CRUD", "Advanced")
    /// </summary>
    string? Group { get; }
}