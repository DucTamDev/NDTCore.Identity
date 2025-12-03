using System.ComponentModel.DataAnnotations;

namespace NDTCore.Identity.Contracts.Features.UserRoles.Requests;

/// <summary>
/// Request model for assigning a role to a user
/// </summary>
public class CreateUserRoleRequest
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

    /// <summary>
    /// Optional: Assignment timestamp (defaults to current time)
    /// </summary>
    public DateTime? AssignedAt { get; set; }

    /// <summary>
    /// Optional: User who assigned the role
    /// </summary>
    [StringLength(200)]
    public string? AssignedBy { get; set; }
}

