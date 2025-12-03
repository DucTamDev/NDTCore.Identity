namespace NDTCore.Identity.Contracts.Authorization;

/// <summary>
/// Defines all system permissions for role-based access control
/// </summary>
public static class Permissions
{
    // ========================================
    // User Management Permissions
    // ========================================
    public static class Users
    {
        public const string View = "Permissions.Users.View";
        public const string Create = "Permissions.Users.Create";
        public const string Edit = "Permissions.Users.Edit";
        public const string Delete = "Permissions.Users.Delete";
        public const string Lock = "Permissions.Users.Lock";
        public const string Unlock = "Permissions.Users.Unlock";
        public const string ResetPassword = "Permissions.Users.ResetPassword";
        public const string ViewSensitiveData = "Permissions.Users.ViewSensitiveData";

        public static List<string> All => new()
        {
            View, Create, Edit, Delete, Lock, Unlock, ResetPassword, ViewSensitiveData
        };
    }

    // ========================================
    // Role Management Permissions
    // ========================================
    public static class Roles
    {
        public const string View = "Permissions.Roles.View";
        public const string Create = "Permissions.Roles.Create";
        public const string Edit = "Permissions.Roles.Edit";
        public const string Delete = "Permissions.Roles.Delete";
        public const string AssignToUser = "Permissions.Roles.AssignToUser";
        public const string RemoveFromUser = "Permissions.Roles.RemoveFromUser";

        public static List<string> All => new()
        {
            View, Create, Edit, Delete, AssignToUser, RemoveFromUser
        };
    }

    // ========================================
    // Role Claims Permissions
    // ========================================
    public static class RoleClaims
    {
        public const string View = "Permissions.RoleClaims.View";
        public const string Create = "Permissions.RoleClaims.Create";
        public const string Delete = "Permissions.RoleClaims.Delete";

        public static List<string> All => new()
        {
            View, Create, Delete
        };
    }

    // ========================================
    // Authentication & Token Permissions
    // ========================================
    public static class Authentication
    {
        public const string Login = "Permissions.Authentication.Login";
        public const string RefreshToken = "Permissions.Authentication.RefreshToken";
        public const string RevokeToken = "Permissions.Authentication.RevokeToken";
        public const string ViewTokens = "Permissions.Authentication.ViewTokens";

        public static List<string> All => new()
        {
            Login, RefreshToken, RevokeToken, ViewTokens
        };
    }

    // ========================================
    // System Administration Permissions
    // ========================================
    public static class SystemAdministration
    {
        public const string ViewAuditLogs = "Permissions.SystemAdministration.ViewAuditLogs";
        public const string ManageSystemSettings = "Permissions.SystemAdministration.ManageSystemSettings";
        public const string ViewHealthChecks = "Permissions.SystemAdministration.ViewHealthChecks";

        public static List<string> All => new()
        {
            ViewAuditLogs, ManageSystemSettings, ViewHealthChecks
        };
    }

    // ========================================
    // Helper Methods
    // ========================================

    /// <summary>
    /// Gets all permissions in the system
    /// </summary>
    public static List<string> GetAllPermissions()
    {
        var allPermissions = new List<string>();
        allPermissions.AddRange(Users.All);
        allPermissions.AddRange(Roles.All);
        allPermissions.AddRange(RoleClaims.All);
        allPermissions.AddRange(Authentication.All);
        allPermissions.AddRange(SystemAdministration.All);
        return allPermissions;
    }

    /// <summary>
    /// Gets permissions grouped by category
    /// </summary>
    public static Dictionary<string, List<string>> GetGroupedPermissions()
    {
        return new Dictionary<string, List<string>>
        {
            { "Users", Users.All },
            { "Roles", Roles.All },
            { "RoleClaims", RoleClaims.All },
            { "Authentication", Authentication.All },
            { "SystemAdministration", SystemAdministration.All }
        };
    }

    /// <summary>
    /// Validates if a permission exists in the system
    /// </summary>
    public static bool IsValidPermission(string permission)
    {
        return GetAllPermissions().Contains(permission);
    }
}