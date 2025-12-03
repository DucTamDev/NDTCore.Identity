using NDTCore.Identity.Contracts.Common;
using NDTCore.Identity.Contracts.Features.Users.DTOs;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Contracts.Interfaces.Repositories;

/// <summary>
/// Repository interface for user management with Result pattern
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Gets a user by their unique identifier
    /// </summary>
    Task<Result<AppUser>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user by email address
    /// </summary>
    Task<Result<AppUser>> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user by username
    /// </summary>
    Task<Result<AppUser>> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all users with pagination and search
    /// </summary>
    Task<Result<PagedResult<AppUser>>> GetAllAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        bool includeDeleted = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all roles assigned to a user
    /// </summary>
    Task<Result<List<string>>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets users by role name
    /// </summary>
    Task<Result<List<AppUser>>> GetUsersByRoleAsync(string roleName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets locked users
    /// </summary>
    Task<Result<List<AppUser>>> GetLockedUsersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets inactive users
    /// </summary>
    Task<Result<List<AppUser>>> GetInactiveUsersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches users by multiple criteria
    /// </summary>
    Task<Result<PagedResult<AppUser>>> SearchUsersAsync(
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
    Task<Result<bool>> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if email is already taken
    /// </summary>
    Task<Result<bool>> EmailExistsAsync(string email, Guid? excludeUserId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if username is already taken
    /// </summary>
    Task<Result<bool>> UserNameExistsAsync(string userName, Guid? excludeUserId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new user
    /// </summary>
    Task<Result<AppUser>> AddAsync(AppUser user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing user
    /// </summary>
    Task<Result<AppUser>> UpdateAsync(AppUser user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Soft deletes a user
    /// </summary>
    Task<Result> DeleteAsync(AppUser user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets user count statistics
    /// </summary>
    Task<Result<UserStatisticsDto>> GetUserStatisticsAsync(CancellationToken cancellationToken = default);
}