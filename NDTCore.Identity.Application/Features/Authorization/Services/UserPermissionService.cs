using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Contracts.Interfaces.Services;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Application.Features.Authorization.Services;

/// <summary>
/// Implementation of user permission service with memory caching
/// </summary>
public sealed class UserPermissionService : IUserPermissionService
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly IMemoryCache _cache;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(15);

    public UserPermissionService(
        IPermissionRepository permissionRepository,
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        IMemoryCache cache)
    {
        _permissionRepository = permissionRepository;
        _userManager = userManager;
        _roleManager = roleManager;
        _cache = cache;
    }

    public async Task<IReadOnlyList<Permission>> GetUserPermissionsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = GetCacheKey(userId);

        if (_cache.TryGetValue(cacheKey, out List<Permission>? cachedPermissions) && cachedPermissions != null)
        {
            return cachedPermissions;
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return Array.Empty<Permission>();

        var roles = await _userManager.GetRolesAsync(user);
        var allPermissions = new List<Permission>();

        foreach (var roleName in roles)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null) continue;

            var permissions = await _permissionRepository.GetByRoleIdAsync(role.Id, cancellationToken);
            if (permissions != null)
            {
                allPermissions.AddRange(permissions);
            }
        }

        // Remove duplicates
        var distinctPermissions = allPermissions
            .GroupBy(p => p.Id)
            .Select(g => g.First())
            .ToList();

        _cache.Set(cacheKey, distinctPermissions, CacheDuration);

        return distinctPermissions;
    }

    public async Task<bool> HasPermissionAsync(
        Guid userId,
        string permissionName,
        CancellationToken cancellationToken = default)
    {
        var permissions = await GetUserPermissionsAsync(userId, cancellationToken);
        return permissions.Any(p => p.Name == permissionName);
    }

    public async Task<bool> HasAnyPermissionAsync(
        Guid userId,
        IEnumerable<string> permissionNames,
        CancellationToken cancellationToken = default)
    {
        var permissions = await GetUserPermissionsAsync(userId, cancellationToken);
        var permissionSet = new HashSet<string>(permissions.Select(p => p.Name));
        return permissionNames.Any(permissionSet.Contains);
    }

    public async Task<bool> HasAllPermissionsAsync(
        Guid userId,
        IEnumerable<string> permissionNames,
        CancellationToken cancellationToken = default)
    {
        var permissions = await GetUserPermissionsAsync(userId, cancellationToken);
        var permissionSet = new HashSet<string>(permissions.Select(p => p.Name));
        return permissionNames.All(permissionSet.Contains);
    }

    public void InvalidateUserCache(Guid userId)
    {
        _cache.Remove(GetCacheKey(userId));
    }

    private static string GetCacheKey(Guid userId) => $"UserPermissions_{userId}";
}