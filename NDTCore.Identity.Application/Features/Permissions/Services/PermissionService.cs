using NDTCore.Identity.Contracts.Common;
using NDTCore.Identity.Contracts.Features.Permissions.DTOs;
using NDTCore.Identity.Contracts.Features.Permissions.Requests;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Contracts.Interfaces.Services;

namespace NDTCore.Identity.Application.Features.Permissions.Services;

/// <summary>
/// Service for handling permission management operations
/// </summary>
public class PermissionService : IPermissionService
{
    private readonly IRoleClaimRepository _roleClaimRepository;
    private readonly IUserClaimRepository _userClaimRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;

    public PermissionService(
        IRoleClaimRepository roleClaimRepository,
        IUserClaimRepository userClaimRepository,
        IUserRepository userRepository,
        IRoleRepository roleRepository)
    {
        _roleClaimRepository = roleClaimRepository;
        _userClaimRepository = userClaimRepository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
    }

    public async Task<ApiResponse<List<PermissionDto>>> GetAllPermissionsAsync(CancellationToken cancellationToken = default)
    {
        var allPermissions = NDTCore.Identity.Contracts.Authorization.Permissions.GetAllPermissions();
        var dtos = allPermissions.Select(p => new PermissionDto
        {
            Name = p,
            Category = ExtractCategory(p),
            Description = GetPermissionDescription(p)
        }).ToList();

        return ApiResponse<List<PermissionDto>>.SuccessResponse(dtos);
    }

    public async Task<ApiResponse<Dictionary<string, List<PermissionDto>>>> GetGroupedPermissionsAsync(CancellationToken cancellationToken = default)
    {
        var grouped = NDTCore.Identity.Contracts.Authorization.Permissions.GetGroupedPermissions();
        var result = new Dictionary<string, List<PermissionDto>>();

        foreach (var group in grouped)
        {
            result[group.Key] = group.Value.Select(p => new PermissionDto
            {
                Name = p,
                Category = group.Key,
                Description = GetPermissionDescription(p)
            }).ToList();
        }

        return ApiResponse<Dictionary<string, List<PermissionDto>>>.SuccessResponse(result);
    }

    public async Task<ApiResponse<UserPermissionsDto>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var userResult = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (userResult.IsFailure)
            return ApiResponse<UserPermissionsDto>.FailureResponse(message: "User not found", statusCode: 404);

        var user = userResult.Value!;

        // Get user roles
        var rolesResult = await _userRepository.GetUserRolesAsync(userId, cancellationToken);
        var roles = rolesResult.IsSuccess ? rolesResult.Value ?? new List<string>() : new List<string>();

        // Get direct user claims (permissions)
        var userClaimsResult = await _userClaimRepository.GetClaimsByUserIdAsync(userId, cancellationToken);
        var directPermissions = userClaimsResult.IsSuccess && userClaimsResult.Value != null
            ? userClaimsResult.Value
                .Where(uc => uc.ClaimType == "Permission")
                .Select(uc => uc.ClaimValue ?? string.Empty)
                .Where(p => !string.IsNullOrEmpty(p))
                .ToList()
            : new List<string>();

        // Get permissions from roles (via RoleClaims)
        var rolePermissions = new List<string>();
        foreach (var roleName in roles)
        {
            var roleResult = await _roleRepository.GetByNameAsync(roleName, cancellationToken);
            if (roleResult.IsSuccess && roleResult.Value != null)
            {
                var roleClaimsResult = await _roleClaimRepository.GetClaimsByRoleIdAsync(roleResult.Value.Id, cancellationToken);
                if (roleClaimsResult.IsSuccess && roleClaimsResult.Value != null)
                {
                    var permissions = roleClaimsResult.Value
                        .Where(rc => rc.ClaimType == "Permission")
                        .Select(rc => rc.ClaimValue ?? string.Empty)
                        .Where(p => !string.IsNullOrEmpty(p));
                    rolePermissions.AddRange(permissions);
                }
            }
        }

        // Combine and deduplicate
        var effectivePermissions = directPermissions
            .Union(rolePermissions)
            .Distinct()
            .OrderBy(p => p)
            .ToList();

        var dto = new UserPermissionsDto
        {
            UserId = user.Id,
            UserName = user.UserName ?? string.Empty,
            Roles = roles,
            DirectPermissions = directPermissions,
            RolePermissions = rolePermissions.Distinct().ToList(),
            EffectivePermissions = effectivePermissions
        };

        return ApiResponse<UserPermissionsDto>.SuccessResponse(dto);
    }

    public async Task<ApiResponse<List<string>>> GetRolePermissionsAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var roleResult = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
        if (roleResult.IsFailure)
            return ApiResponse<List<string>>.FailureResponse(message: "Role not found", statusCode: 404);

        var claimsResult = await _roleClaimRepository.GetClaimsByRoleIdAsync(roleId, cancellationToken);
        if (claimsResult.IsFailure)
            return ApiResponse<List<string>>.FailureResponse(message: "Failed to retrieve role claims", statusCode: 500);

        var permissions = claimsResult.Value!
            .Where(rc => rc.ClaimType == "Permission")
            .Select(rc => rc.ClaimValue ?? string.Empty)
            .Where(p => !string.IsNullOrEmpty(p))
            .Distinct()
            .OrderBy(p => p)
            .ToList();

        return ApiResponse<List<string>>.SuccessResponse(permissions);
    }

    public async Task<ApiResponse> AssignPermissionsToRoleAsync(AssignPermissionRequest request, CancellationToken cancellationToken = default)
    {
        var roleResult = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
        if (roleResult.IsFailure)
            return ApiResponse.FailureResponse(message: "Role not found", statusCode: 404);

        var role = roleResult.Value!;

        // Validate all permissions exist
        var invalidPermissions = request.Permissions.Where(p => !NDTCore.Identity.Contracts.Authorization.Permissions.IsValidPermission(p)).ToList();
        if (invalidPermissions.Any())
        {
            return ApiResponse.FailureResponse(
                $"Invalid permissions: {string.Join(", ", invalidPermissions)}",
                400);
        }

        // Add permissions as RoleClaims
        var roleClaims = request.Permissions.Select(permission => new Domain.Entities.AppRoleClaim
        {
            RoleId = role.Id,
            ClaimType = "Permission",
            ClaimValue = permission
        }).ToList();

        var addResult = await _roleClaimRepository.AddRangeAsync(roleClaims, cancellationToken);
        if (addResult.IsFailure)
            return addResult.ToApiResponse(failureStatusCode: 500);

        return ApiResponse.SuccessResponse($"Assigned {request.Permissions.Count} permission(s) to role successfully");
    }

    public async Task<ApiResponse> RevokePermissionsFromRoleAsync(Guid roleId, List<string> permissions, CancellationToken cancellationToken = default)
    {
        var roleResult = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
        if (roleResult.IsFailure)
            return ApiResponse.FailureResponse(message: "Role not found", statusCode: 404);

        var claimsResult = await _roleClaimRepository.GetClaimsByRoleIdAsync(roleId, cancellationToken);
        if (claimsResult.IsFailure)
            return ApiResponse.FailureResponse(message: "Failed to retrieve role claims", statusCode: 500);

        var claimsToRemove = claimsResult.Value!
            .Where(rc => rc.ClaimType == "Permission" && permissions.Contains(rc.ClaimValue ?? string.Empty))
            .ToList();

        foreach (var claim in claimsToRemove)
        {
            var deleteResult = await _roleClaimRepository.DeleteAsync(claim, cancellationToken);
            if (deleteResult.IsFailure)
                return deleteResult.ToApiResponse(failureStatusCode: 500);
        }

        return ApiResponse.SuccessResponse($"Revoked {claimsToRemove.Count} permission(s) from role successfully");
    }

    private string ExtractCategory(string permission)
    {
        // Extract category from permission name (e.g., "Permissions.Users.View" -> "Users")
        var parts = permission.Split('.');
        return parts.Length > 1 ? parts[1] : "Other";
    }

    private string? GetPermissionDescription(string permission)
    {
        // Map permission to description (can be enhanced with a dictionary)
        return permission switch
        {
            var p when p.Contains(".View") => "View access",
            var p when p.Contains(".Create") => "Create access",
            var p when p.Contains(".Edit") => "Edit access",
            var p when p.Contains(".Delete") => "Delete access",
            _ => null
        };
    }
}

