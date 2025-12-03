using System.ComponentModel.DataAnnotations;

namespace NDTCore.Identity.Contracts.Features.Permissions.Requests;

/// <summary>
/// Request model for assigning permissions to a role
/// </summary>
public class AssignPermissionRequest
{
    /// <summary>
    /// Role ID
    /// </summary>
    [Required(ErrorMessage = "Role ID is required")]
    public Guid RoleId { get; set; }

    /// <summary>
    /// List of permission names to assign
    /// </summary>
    [Required(ErrorMessage = "At least one permission is required")]
    [MinLength(1, ErrorMessage = "At least one permission must be specified")]
    public List<string> Permissions { get; set; } = new();
}

