using NDTCore.Identity.Contracts.Common;
using NDTCore.Identity.Contracts.Features.UserRoles.DTOs;
using NDTCore.Identity.Contracts.Features.UserRoles.Requests;

namespace NDTCore.Identity.Contracts.Interfaces.Services;

/// <summary>
/// Service interface for user-role assignment management
/// </summary>
public interface IUserRoleService
{
    /// <summary>
    /// Gets user-role assignment by user ID and role ID
    /// </summary>
    Task<ApiResponse<UserRoleDto>> GetUserRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all role assignments for a user
    /// </summary>
    Task<ApiResponse<List<UserRoleDto>>> GetUserRolesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all user assignments for a role
    /// </summary>
    Task<ApiResponse<List<UserRoleDto>>> GetUserRolesByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets paginated list of user-role assignments
    /// </summary>
    Task<ApiResponse<PagedResult<UserRoleDto>>> GetUserRolesAsync(GetUserRolesRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigns a role to a user
    /// </summary>
    Task<ApiResponse<UserRoleDto>> AssignRoleToUserAsync(CreateUserRoleRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates user-role assignment metadata
    /// </summary>
    Task<ApiResponse<UserRoleDto>> UpdateUserRoleAsync(Guid userId, Guid roleId, UpdateUserRoleRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a role from a user
    /// </summary>
    Task<ApiResponse> RemoveRoleFromUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
}

