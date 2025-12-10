using NDTCore.Identity.Contracts.Common.Pagination;
using NDTCore.Identity.Contracts.Features.Users.DTOs;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Contracts.Interfaces.Repositories;

/// <summary>
/// Repository interface for user management
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Gets a user by their unique identifier
    /// </summary>
    Task<AppUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user by email address
    /// </summary>
    Task<AppUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user by username
    /// </summary>
    Task<AppUser?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all users with pagination and search
    /// </summary>
    Task<PaginatedCollection<AppUser>> GetAllAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        bool includeDeleted = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all roles assigned to a user
    /// </summary>
    Task<List<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets users by role name
    /// </summary>
    Task<List<AppUser>> GetUsersByRoleAsync(string roleName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets locked users
    /// </summary>
    Task<List<AppUser>> GetLockedUsersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets inactive users
    /// </summary>
    Task<List<AppUser>> GetInactiveUsersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches users by multiple criteria
    /// </summary>
    Task<PaginatedCollection<AppUser>> SearchUsersAsync(
        string? searchTerm,
        bool? isActive,
        bool? isLocked,
        DateTime? createdAfter,
        DateTime? createdBefore,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user exists by ID
    /// </summary>
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if email is already taken
    /// </summary>
    Task<bool> EmailExistsAsync(string email, Guid? excludeUserId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if username is already taken
    /// </summary>
    Task<bool> UserNameExistsAsync(string userName, Guid? excludeUserId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new user
    /// </summary>
    Task<AppUser> AddAsync(AppUser user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing user
    /// </summary>
    Task<AppUser> UpdateAsync(AppUser user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Soft deletes a user
    /// </summary>
    Task DeleteAsync(AppUser user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets user count statistics
    /// </summary>
    Task<UserStatisticsDto> GetUserStatisticsAsync(CancellationToken cancellationToken = default);
}
