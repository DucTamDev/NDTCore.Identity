namespace NDTCore.Identity.Contracts.Features.Permissions.DTOs;

/// <summary>
/// Permission data transfer object
/// </summary>
public class PermissionDto
{
    /// <summary>
    /// Permission name (e.g., "Permissions.Users.View")
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Permission category (e.g., "Users", "Roles")
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Permission description
    /// </summary>
    public string? Description { get; set; }
}

