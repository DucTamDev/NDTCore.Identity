namespace NDTCore.Identity.Contracts.Features.Roles.DTOs;

/// <summary>
/// Role data transfer object
/// </summary>
public class RoleDto
{
    /// <summary>
    /// Role unique identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Role name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Role description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Role priority (higher number = higher priority)
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// Indicates if this is a system role (cannot be modified/deleted)
    /// </summary>
    public bool IsSystemRole { get; set; }

    /// <summary>
    /// Role creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

