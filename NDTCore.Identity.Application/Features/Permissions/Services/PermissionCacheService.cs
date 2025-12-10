using Microsoft.Extensions.Caching.Memory;

namespace NDTCore.Identity.Application.Features.Permissions.Services;

/// <summary>
/// Permission caching service for improved performance
/// </summary>
public class PermissionCacheService
{
    private readonly IMemoryCache _cache;
    private const string PermissionCacheKeyPrefix = "Permission_";
    private const string UserPermissionsCacheKeyPrefix = "UserPermissions_";
    private const int CacheExpirationMinutes = 30;

    public PermissionCacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public T? GetFromCache<T>(string key)
    {
        return _cache.TryGetValue(key, out T? value) ? value : default;
    }

    public void SetCache<T>(string key, T value)
    {
        var options = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));

        _cache.Set(key, value, options);
    }

    public void InvalidateCache(string key)
    {
        _cache.Remove(key);
    }

    public void InvalidateUserPermissionsCache(Guid userId)
    {
        _cache.Remove($"{UserPermissionsCacheKeyPrefix}{userId}");
    }

    public string GetPermissionCacheKey(string permissionName)
    {
        return $"{PermissionCacheKeyPrefix}{permissionName}";
    }

    public string GetUserPermissionsCacheKey(Guid userId)
    {
        return $"{UserPermissionsCacheKeyPrefix}{userId}";
    }
}

