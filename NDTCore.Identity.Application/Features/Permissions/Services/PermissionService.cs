using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Authorization.Permissions;
using NDTCore.Identity.Contracts.Common.Results;
using NDTCore.Identity.Contracts.Features.Permissions.DTOs;
using NDTCore.Identity.Contracts.Features.Permissions.Requests;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Contracts.Interfaces.Services;
using NDTCore.Identity.Domain.Constants;
using NDTCore.Identity.Domain.Exceptions;

namespace NDTCore.Identity.Application.Features.Permissions.Services;

/// <summary>
/// Service for handling permission management operations
/// </summary>
public class PermissionService : IPermissionService
{
    private readonly ILogger<PermissionService> _logger;
    private readonly IRoleClaimRepository _roleClaimRepository;
    private readonly IUserClaimRepository _userClaimRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRegistry _permissionRegistry;

    public PermissionService(
        ILogger<PermissionService> logger,
        IRoleClaimRepository roleClaimRepository,
        IUserClaimRepository userClaimRepository,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IPermissionRegistry permissionRegistry)
    {
        _logger = logger;
        _roleClaimRepository = roleClaimRepository;
        _userClaimRepository = userClaimRepository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _permissionRegistry = permissionRegistry;
    }

    public async Task<Result<List<PermissionDto>>> GetAllPermissionsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var allPermissions = _permissionRegistry.GetAllPermissions();
            var dtos = allPermissions.Select(p => new PermissionDto
            {
                Name = p.Name,
                Module = p.Module,
                Description = p.Description
            }).ToList();

            return Result<List<PermissionDto>>.Success(dtos, "Permissions retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all permissions");
            return Result<List<PermissionDto>>.InternalError("An error occurred while retrieving permissions");
        }
    }

    public async Task<Result<Dictionary<string, List<PermissionDto>>>> GetGroupedPermissionsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var grouped = _permissionRegistry.GetPermissionsByModule();
            var result = new Dictionary<string, List<PermissionDto>>();

            foreach (var group in grouped)
            {
                result[group.Key] = group.Value.Select(p => new PermissionDto
                {
                    Name = p.Name,
                    Module = group.Key,
                    Description = p.Description
                }).ToList();
            }

            return Result<Dictionary<string, List<PermissionDto>>>.Success(result, "Grouped permissions retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting grouped permissions");

            return Result<Dictionary<string, List<PermissionDto>>>.InternalError("An error occurred while retrieving grouped permissions");
        }
    }

    public async Task<Result<UserPermissionsDto>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
                return Result<UserPermissionsDto>.NotFound($"User with ID '{userId}' was not found");

            // Get user roles
            var roles = await _userRepository.GetUserRolesAsync(userId, cancellationToken);

            // Get direct user claims (permissions)
            var userClaims = await _userClaimRepository.GetClaimsByUserIdAsync(userId, cancellationToken);
            var directPermissions = userClaims
                .Where(uc => uc.ClaimType == ClaimTypes.Permission)
                .Select(uc => uc.ClaimValue ?? string.Empty)
                .Where(p => !string.IsNullOrEmpty(p))
                .ToList();

            // Get permissions from roles (via RoleClaims)
            var rolePermissions = new List<string>();
            foreach (var roleName in roles)
            {
                var role = await _roleRepository.GetByNameAsync(roleName, cancellationToken);
                if (role != null)
                {
                    var roleClaims = await _roleClaimRepository.GetClaimsByRoleIdAsync(role.Id, cancellationToken);
                    var permissions = roleClaims
                        .Where(rc => rc.ClaimType == ClaimTypes.Permission)
                        .Select(rc => rc.ClaimValue ?? string.Empty)
                        .Where(p => !string.IsNullOrEmpty(p));
                    rolePermissions.AddRange(permissions);
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

            return Result<UserPermissionsDto>.Success(dto, "User permissions retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user permissions: {UserId}", userId);
            return Result<UserPermissionsDto>.InternalError("An error occurred while retrieving user permissions");
        }
    }

    public async Task<Result<List<string>>> GetRolePermissionsAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
            if (role == null)
                return Result<List<string>>.NotFound($"Role with ID '{roleId}' was not found");

            var claims = await _roleClaimRepository.GetClaimsByRoleIdAsync(roleId, cancellationToken);

            var permissions = claims
                .Where(rc => rc.ClaimType == ClaimTypes.Permission)
                .Select(rc => rc.ClaimValue ?? string.Empty)
                .Where(p => !string.IsNullOrEmpty(p))
                .Distinct()
                .OrderBy(p => p)
                .ToList();

            return Result<List<string>>.Success(permissions, "Role permissions retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting role permissions: {RoleId}", roleId);

            return Result<List<string>>.InternalError("An error occurred while retrieving role permissions");
        }
    }

    public async Task<Result> AssignPermissionsToRoleAsync(AssignPermissionRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
            if (role == null)
                return Result.NotFound($"Role with ID '{request.RoleId}' was not found");

            // Validate all permissions exist
            var invalidPermissions = request.Permissions.Where(p => !_permissionRegistry.IsValidPermission(p)).ToList();
            if (invalidPermissions.Count != 0)
            {
                return Result.BadRequest(
                    message: $"Invalid permissions: {string.Join(", ", invalidPermissions)}",
                    errorCode: ErrorCodes.ValidationError);
            }

            // Add permissions as RoleClaims
            var roleClaims = request.Permissions.Select(permission => new Domain.Entities.AppRoleClaim
            {
                RoleId = role.Id,
                ClaimType = ClaimTypes.Permission,
                ClaimValue = permission
            }).ToList();

            await _roleClaimRepository.AddRangeAsync(roleClaims, cancellationToken);

            return Result.Success($"Assigned {request.Permissions.Count} permission(s) to role successfully");
        }
        catch (ConflictException ex)
        {
            return Result.Conflict(ex.Message);
        }
        catch (DomainException ex)
        {
            return Result.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning permissions to role: {RoleId}", request.RoleId);
            return Result.InternalError("An error occurred while assigning permissions");
        }
    }

    public async Task<Result> RevokePermissionsFromRoleAsync(Guid roleId, List<string> permissions, CancellationToken cancellationToken = default)
    {
        try
        {
            var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
            if (role == null)
                return Result.NotFound($"Role with ID '{roleId}' was not found");

            var claims = await _roleClaimRepository.GetClaimsByRoleIdAsync(roleId, cancellationToken);
            var claimsToRemove = claims
                .Where(rc => rc.ClaimType == ClaimTypes.Permission && permissions.Contains(rc.ClaimValue ?? string.Empty))
                .ToList();

            foreach (var claim in claimsToRemove)
            {
                await _roleClaimRepository.DeleteAsync(claim, cancellationToken);
            }

            return Result.Success($"Revoked {claimsToRemove.Count} permission(s) from role successfully");
        }
        catch (ConflictException ex)
        {
            return Result.Conflict(ex.Message);
        }
        catch (DomainException ex)
        {
            return Result.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking permissions from role: {RoleId}", roleId);
            return Result.InternalError("An error occurred while revoking permissions");
        }
    }
}