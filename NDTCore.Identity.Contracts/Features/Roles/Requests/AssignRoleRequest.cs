using System.ComponentModel.DataAnnotations;

namespace NDTCore.Identity.Contracts.Features.Roles.Requests;

/// <summary>
/// Request model for assigning a role to a user
/// </summary>
public class AssignRoleRequest
{
    /// <summary>
    /// User ID
    /// </summary>
    [Required(ErrorMessage = "User ID is required")]
    public Guid UserId { get; set; }

    /// <summary>
    /// Role ID
    /// </summary>
    [Required(ErrorMessage = "Role ID is required")]
    public Guid RoleId { get; set; }
}

