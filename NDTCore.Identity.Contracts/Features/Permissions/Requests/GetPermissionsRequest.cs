namespace NDTCore.Identity.Contracts.Features.Permissions.Requests;

/// <summary>
/// Request model for getting permissions
/// </summary>
public class GetPermissionsRequest
{
    /// <summary>
    /// Optional: Filter by user ID to get user's effective permissions
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Optional: Filter by role ID to get role's permissions
    /// </summary>
    public Guid? RoleId { get; set; }
}

