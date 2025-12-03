namespace NDTCore.Identity.Contracts.Features.UserRoles.DTOs;

/// <summary>
/// User-role assignment data transfer object
/// </summary>
public class UserRoleDto
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
    /// User email
    /// </summary>
    public string UserEmail { get; set; } = string.Empty;

    /// <summary>
    /// User full name
    /// </summary>
    public string UserFullName { get; set; } = string.Empty;

    /// <summary>
    /// Role ID
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// Role name
    /// </summary>
    public string RoleName { get; set; } = string.Empty;

    /// <summary>
    /// Role description
    /// </summary>
    public string? RoleDescription { get; set; }

    /// <summary>
    /// Assignment timestamp
    /// </summary>
    public DateTime AssignedAt { get; set; }

    /// <summary>
    /// User who assigned the role
    /// </summary>
    public string? AssignedBy { get; set; }
}

