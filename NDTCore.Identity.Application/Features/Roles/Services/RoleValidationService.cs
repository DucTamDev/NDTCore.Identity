namespace NDTCore.Identity.Application.Features.Roles.Services;

/// <summary>
/// Role-specific business validation service
/// </summary>
public class RoleValidationService
{
    public bool IsValidRoleName(string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
            return false;

        // Role name must be alphanumeric and can contain spaces
        return roleName.All(c => char.IsLetterOrDigit(c) || c == ' ');
    }

    public bool IsSystemRole(string roleName)
    {
        var systemRoles = new[] { "Admin", "SuperAdmin", "System" };
        return systemRoles.Contains(roleName, StringComparer.OrdinalIgnoreCase);
    }
}

