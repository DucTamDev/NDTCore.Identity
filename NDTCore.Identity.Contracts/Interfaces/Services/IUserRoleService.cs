using NDTCore.Identity.Contracts.Common.Pagination;
using NDTCore.Identity.Contracts.Common.Results;
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
    Task<Result<UserRoleDto>> GetUserRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all role assignments for a user
    /// </summary>
    Task<Result<List<UserRoleDto>>> GetUserRolesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all user assignments for a role
    /// </summary>
    Task<Result<List<UserRoleDto>>> GetUserRolesByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets paginated list of user-role assignments
    /// </summary>
    Task<Result<PaginatedCollection<UserRoleDto>>> GetUserRolesAsync(GetUserRolesRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigns a role to a user
    /// </summary>
    Task<Result<UserRoleDto>> AssignRoleToUserAsync(CreateUserRoleRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates user-role assignment metadata
    /// </summary>
    Task<Result<UserRoleDto>> UpdateUserRoleAsync(Guid userId, Guid roleId, UpdateUserRoleRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a role from a user
    /// </summary>
    Task<Result> RemoveRoleFromUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
}

