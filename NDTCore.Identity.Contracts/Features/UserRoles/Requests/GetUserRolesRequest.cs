using System.ComponentModel.DataAnnotations;

namespace NDTCore.Identity.Contracts.Features.UserRoles.Requests;

/// <summary>
/// Request model for getting paginated list of user-role assignments
/// </summary>
public class GetUserRolesRequest
{
    /// <summary>
    /// Optional: Filter by user ID
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Optional: Filter by role ID
    /// </summary>
    public Guid? RoleId { get; set; }

    /// <summary>
    /// Page number (1-based)
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0")]
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Page size
    /// </summary>
    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
    public int PageSize { get; set; } = 10;
}

