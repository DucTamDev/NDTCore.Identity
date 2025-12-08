using System.Collections.Concurrent;

namespace NDTCore.Identity.Contracts.Authorization.Permissions;

/// <summary>
/// Thread-safe implementation of the permission registry
/// </summary>
public sealed class PermissionRegistry : IPermissionRegistry
{
    private readonly ConcurrentDictionary<string, IPermissionDefinition> _permissions = new();
    private readonly ConcurrentDictionary<string, PermissionModule> _modules = new();
    private readonly object _lock = new();

    public void RegisterModule(PermissionModule module)
    {
        lock (_lock)
        {
            _modules[module.Name] = module;

            foreach (var permission in module.Permissions)
            {
                _permissions[permission.Name] = permission;
            }
        }
    }

    public void RegisterPermission(IPermissionDefinition permission)
    {
        _permissions[permission.Name] = permission;
    }

    public IReadOnlyList<IPermissionDefinition> GetAllPermissions()
    {
        return _permissions.Values.OrderBy(p => p.Module)
                                   .ThenBy(p => p.SortOrder)
                                   .ThenBy(p => p.DisplayName)
                                   .ToList();
    }

    public IReadOnlyDictionary<string, List<IPermissionDefinition>> GetPermissionsByModule()
    {
        return _permissions.Values
            .GroupBy(p => p.Module)
            .ToDictionary(
                g => g.Key,
                g => g.OrderBy(p => p.SortOrder)
                      .ThenBy(p => p.DisplayName)
                      .ToList()
            );
    }

    public IReadOnlyList<PermissionModule> GetAllModules()
    {
        return _modules.Values.OrderBy(m => m.SortOrder)
                               .ThenBy(m => m.DisplayName)
                               .ToList();
    }

    public IPermissionDefinition? GetPermission(string name)
    {
        _permissions.TryGetValue(name, out var permission);
        return permission;
    }

    public IReadOnlyList<IPermissionDefinition> GetModulePermissions(string moduleName)
    {
        return _permissions.Values
            .Where(p => p.Module.Equals(moduleName, StringComparison.OrdinalIgnoreCase))
            .OrderBy(p => p.SortOrder)
            .ThenBy(p => p.DisplayName)
            .ToList();
    }

    public bool IsValidPermission(string permissionName)
    {
        return _permissions.ContainsKey(permissionName);
    }
}