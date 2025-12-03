using NDTCore.Identity.Contracts.Common;
using NDTCore.Identity.Contracts.Features.Roles.DTOs;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Contracts.Interfaces.Repositories;

/// <summary>
/// Repository interface for role management with Result pattern
/// </summary>
public interface IRoleRepository
{
    /// <summary>
    /// Gets a role by its unique identifier
    /// </summary>
    Task<Result<AppRole>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a role by name
    /// </summary>
    Task<Result<AppRole>> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all roles in the system
    /// </summary>
    Task<Result<List<AppRole>>> GetAllAsync(bool includeSystemRoles = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets roles with pagination
    /// </summary>
    Task<Result<PagedResult<AppRole>>> GetAllPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets system roles only
    /// </summary>
    Task<Result<List<AppRole>>> GetSystemRolesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets custom (non-system) roles only
    /// </summary>
    Task<Result<List<AppRole>>> GetCustomRolesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets roles with their associated claims
    /// </summary>
    Task<Result<AppRole>> GetRoleWithClaimsAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets users count for a specific role
    /// </summary>
    Task<Result<int>> GetUserCountInRoleAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a role exists by ID
    /// </summary>
    Task<Result<bool>> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a role name is already taken
    /// </summary>
    Task<Result<bool>> RoleNameExistsAsync(string name, Guid? excludeRoleId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a role is a system role
    /// </summary>
    Task<Result<bool>> IsSystemRoleAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new role
    /// </summary>
    Task<Result<AppRole>> AddAsync(AppRole role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing role
    /// </summary>
    Task<Result<AppRole>> UpdateAsync(AppRole role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a role (only if not system role and no users assigned)
    /// </summary>
    Task<Result> DeleteAsync(AppRole role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets role statistics
    /// </summary>
    Task<Result<RoleStatisticsDto>> GetRoleStatisticsAsync(CancellationToken cancellationToken = default);
}
