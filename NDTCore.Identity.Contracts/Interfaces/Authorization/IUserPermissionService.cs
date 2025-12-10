using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Contracts.Interfaces.Authorization;

/// <summary>
/// Service for efficient user permission checks with caching support
/// </summary>
public interface IUserPermissionService
{
    /// <summary>
    /// Gets all permissions for a user (including role-based permissions)
    /// </summary>
    Task<IReadOnlyList<Permission>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user has a specific permission
    /// </summary>
    Task<bool> HasPermissionAsync(Guid userId, string permissionName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user has any of the specified permissions
    /// </summary>
    Task<bool> HasAnyPermissionAsync(Guid userId, IEnumerable<string> permissionNames, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user has all of the specified permissions
    /// </summary>
    Task<bool> HasAllPermissionsAsync(Guid userId, IEnumerable<string> permissionNames, CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalidates the permission cache for a user
    /// </summary>
    void InvalidateUserCache(Guid userId);
}
