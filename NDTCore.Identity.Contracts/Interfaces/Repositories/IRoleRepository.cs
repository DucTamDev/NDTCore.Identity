using NDTCore.Identity.Contracts.Common.Pagination;
using NDTCore.Identity.Contracts.Features.Roles.DTOs;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Contracts.Interfaces.Repositories;

/// <summary>
/// Repository interface for role management
/// </summary>
public interface IRoleRepository
{
    /// <summary>
    /// Gets a role by its unique identifier
    /// </summary>
    Task<AppRole?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a role by name
    /// </summary>
    Task<AppRole?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all roles in the system
    /// </summary>
    Task<List<AppRole>> GetAllAsync(bool includeSystemRoles = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets roles with pagination
    /// </summary>
    Task<PaginatedCollection<AppRole>> GetAllPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets system roles only
    /// </summary>
    Task<List<AppRole>> GetSystemRolesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets custom (non-system) roles only
    /// </summary>
    Task<List<AppRole>> GetCustomRolesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets roles with their associated claims
    /// </summary>
    Task<AppRole?> GetRoleWithClaimsAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets users count for a specific role
    /// </summary>
    Task<int> GetUserCountInRoleAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a role exists by ID
    /// </summary>
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a role name is already taken
    /// </summary>
    Task<bool> RoleNameExistsAsync(string name, Guid? excludeRoleId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a role is a system role
    /// </summary>
    Task<bool> IsSystemRoleAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new role
    /// </summary>
    Task<AppRole> AddAsync(AppRole role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing role
    /// </summary>
    Task<AppRole> UpdateAsync(AppRole role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a role (only if not system role and no users assigned)
    /// </summary>
    Task DeleteAsync(AppRole role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets role statistics
    /// </summary>
    Task<RoleStatisticsDto> GetRoleStatisticsAsync(CancellationToken cancellationToken = default);
}
