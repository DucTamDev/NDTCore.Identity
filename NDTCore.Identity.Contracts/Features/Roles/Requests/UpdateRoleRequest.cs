using System.ComponentModel.DataAnnotations;

namespace NDTCore.Identity.Contracts.Features.Roles.Requests;

/// <summary>
/// Request model for updating an existing role
/// </summary>
public class UpdateRoleRequest
{
    /// <summary>
    /// Role name
    /// </summary>
    [Required(ErrorMessage = "Role name is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Role name must be between 3 and 50 characters")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Role description (optional)
    /// </summary>
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }

    /// <summary>
    /// Role priority (higher number = higher priority)
    /// </summary>
    public int Priority { get; set; }
}

