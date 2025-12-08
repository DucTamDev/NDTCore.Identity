using Microsoft.AspNetCore.Authorization;

namespace NDTCore.Identity.Contracts.Authorization.Requirements;

/// <summary>
/// Authorization requirement that succeeds when the user has ANY of the specified permissions (OR logic)
/// </summary>
public sealed class HasAnyPermissionRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// The list of permissions (user needs at least one)
    /// </summary>
    public IReadOnlyList<string> Permissions { get; }

    public HasAnyPermissionRequirement(params string[] permissions)
    {
        if (permissions == null || permissions.Length == 0)
            throw new ArgumentException("At least one permission must be specified", nameof(permissions));

        Permissions = permissions.ToList().AsReadOnly();
    }

    public HasAnyPermissionRequirement(IEnumerable<string> permissions)
        : this(permissions.ToArray())
    {
    }

    public override string ToString() => $"HasAnyPermission: [{string.Join(", ", Permissions)}]";
}

