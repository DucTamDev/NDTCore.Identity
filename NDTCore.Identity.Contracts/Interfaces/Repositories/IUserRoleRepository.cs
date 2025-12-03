using NDTCore.Identity.Contracts.Common;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Contracts.Interfaces.Repositories;

/// <summary>
/// Repository interface for user-role assignment management
/// </summary>
public interface IUserRoleRepository
{
    /// <summary>
    /// Gets a user-role assignment by user ID and role ID
    /// </summary>
    Task<Result<AppUserRole>> GetUserRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all role assignments for a user
    /// </summary>
    Task<Result<List<AppUserRole>>> GetUserRolesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all user assignments for a role
    /// </summary>
    Task<Result<List<AppUserRole>>> GetUserRolesByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets paginated list of user-role assignments
    /// </summary>
    Task<Result<PagedResult<AppUserRole>>> GetUserRolesAsync(
        int pageNumber,
        int pageSize,
        Guid? userId = null,
        Guid? roleId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user has a specific role
    /// </summary>
    Task<Result<bool>> UserHasRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a user-role assignment
    /// </summary>
    Task<Result<AppUserRole>> AddAsync(AppUserRole userRole, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a user-role assignment
    /// </summary>
    Task<Result<AppUserRole>> UpdateAsync(AppUserRole userRole, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a user-role assignment
    /// </summary>
    Task<Result> RemoveAsync(AppUserRole userRole, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes all roles from a user
    /// </summary>
    Task<Result> RemoveAllUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes all users from a role
    /// </summary>
    Task<Result> RemoveAllRoleUsersAsync(Guid roleId, CancellationToken cancellationToken = default);
}

