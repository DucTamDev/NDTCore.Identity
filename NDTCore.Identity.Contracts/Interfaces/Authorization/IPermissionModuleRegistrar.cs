using NDTCore.Identity.Contracts.Authorization.Permissions;

namespace NDTCore.Identity.Contracts.Interfaces.Authorization;

/// <summary>
/// Central registry for all permissions in the system
/// </summary>
public interface IPermissionModuleRegistrar
{
    /// <summary>
    /// Registers a permission module
    /// </summary>
    void RegisterModule(PermissionModule module);

    /// <summary>
    /// Registers a single permission
    /// </summary>
    void RegisterPermission(IPermissionDefinition permission);

    /// <summary>
    /// Gets all registered permissions
    /// </summary>
    IReadOnlyList<IPermissionDefinition> GetAllPermissions();

    /// <summary>
    /// Gets permissions grouped by module
    /// </summary>
    IReadOnlyDictionary<string, List<IPermissionDefinition>> GetPermissionsByModule();

    /// <summary>
    /// Gets all registered modules
    /// </summary>
    IReadOnlyList<PermissionModule> GetAllModules();

    /// <summary>
    /// Gets a specific permission by name
    /// </summary>
    IPermissionDefinition? GetPermission(string name);

    /// <summary>
    /// Gets permissions for a specific module
    /// </summary>
    IReadOnlyList<IPermissionDefinition> GetModulePermissions(string moduleName);

    /// <summary>
    /// Checks if a permission exists
    /// </summary>
    bool IsValidPermission(string permissionName);
}
