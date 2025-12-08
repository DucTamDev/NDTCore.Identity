using Microsoft.AspNetCore.Authorization;

namespace NDTCore.Identity.Contracts.Authorization.Requirements;

/// <summary>
/// Authorization requirement for a single permission
/// </summary>
public sealed class PermissionRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// The required permission name
    /// </summary>
    public string Permission { get; }

    public PermissionRequirement(string permission)
    {
        if (string.IsNullOrWhiteSpace(permission))
            throw new ArgumentNullException(nameof(permission));

        Permission = permission;
    }

    public override string ToString() => $"Permission: {Permission}";
}

