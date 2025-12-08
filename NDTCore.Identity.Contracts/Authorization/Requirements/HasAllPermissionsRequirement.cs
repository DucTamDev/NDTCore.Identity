using Microsoft.AspNetCore.Authorization;

namespace NDTCore.Identity.Contracts.Authorization.Requirements;

/// <summary>
/// Authorization requirement that succeeds when the user has ALL of the specified permissions (AND logic)
/// </summary>
public sealed class HasAllPermissionsRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// The list of permissions (user needs all of them)
    /// </summary>
    public IReadOnlyList<string> Permissions { get; }

    public HasAllPermissionsRequirement(params string[] permissions)
    {
        if (permissions == null || permissions.Length == 0)
            throw new ArgumentException("At least one permission must be specified", nameof(permissions));

        Permissions = permissions.ToList().AsReadOnly();
    }

    public HasAllPermissionsRequirement(IEnumerable<string> permissions)
        : this(permissions.ToArray())
    {
    }

    public override string ToString() => $"HasAllPermissions: [{string.Join(", ", Permissions)}]";
}

