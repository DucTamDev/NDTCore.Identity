using System.ComponentModel.DataAnnotations;

namespace NDTCore.Identity.Contracts.Features.UserRoles.Requests;

/// <summary>
/// Request model for updating user-role assignment metadata
/// </summary>
public class UpdateUserRoleRequest
{
    /// <summary>
    /// Assignment timestamp
    /// </summary>
    public DateTime? AssignedAt { get; set; }

    /// <summary>
    /// User who assigned the role
    /// </summary>
    [StringLength(200)]
    public string? AssignedBy { get; set; }
}

