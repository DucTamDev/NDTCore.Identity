using NDTCore.Identity.Contracts.Common.Results;
using NDTCore.Identity.Contracts.Features.Permissions.DTOs;
using NDTCore.Identity.Contracts.Features.Permissions.Requests;

namespace NDTCore.Identity.Contracts.Interfaces.Services;

/// <summary>
/// Service interface for permission management
/// </summary>
public interface IPermissionService
{
    /// <summary>
    /// Gets all available permissions in the system
    /// </summary>
    Task<Result<List<PermissionDto>>> GetAllPermissionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets permissions grouped by category
    /// </summary>
    Task<Result<Dictionary<string, List<PermissionDto>>>> GetGroupedPermissionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets effective permissions for a user (direct + role permissions)
    /// </summary>
    Task<Result<UserPermissionsDto>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets permissions assigned to a role
    /// </summary>
    Task<Result<List<string>>> GetRolePermissionsAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigns permissions to a role (via RoleClaims)
    /// </summary>
    Task<Result> AssignPermissionsToRoleAsync(AssignPermissionRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes permissions from a role
    /// </summary>
    Task<Result> RevokePermissionsFromRoleAsync(Guid roleId, List<string> permissions, CancellationToken cancellationToken = default);
}

