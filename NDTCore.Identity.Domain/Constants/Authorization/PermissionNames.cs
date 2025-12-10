namespace NDTCore.Identity.Domain.Constants.Authorization;

/// <summary>
/// Domain constants for permission names used throughout the system
/// These represent the ubiquitous language of the domain
/// </summary>
public static class PermissionNames
{
    /// <summary>
    /// User management permissions
    /// </summary>
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
    }

    /// <summary>
    /// Role management permissions
    /// </summary>
    public static class Roles
    {
        public const string View = "Permissions.Roles.View";
        public const string Create = "Permissions.Roles.Create";
        public const string Edit = "Permissions.Roles.Edit";
        public const string Delete = "Permissions.Roles.Delete";
        public const string AssignToUser = "Permissions.Roles.AssignToUser";
        public const string RemoveFromUser = "Permissions.Roles.RemoveFromUser";
    }

    /// <summary>
    /// Role claims permissions
    /// </summary>
    public static class RoleClaims
    {
        public const string View = "Permissions.RoleClaims.View";
        public const string Create = "Permissions.RoleClaims.Create";
        public const string Delete = "Permissions.RoleClaims.Delete";
    }

    /// <summary>
    /// Authentication & token permissions
    /// </summary>
    public static class Authentication
    {
        public const string Login = "Permissions.Authentication.Login";
        public const string RefreshToken = "Permissions.Authentication.RefreshToken";
        public const string RevokeToken = "Permissions.Authentication.RevokeToken";
        public const string ViewTokens = "Permissions.Authentication.ViewTokens";
    }

    /// <summary>
    /// System administration permissions
    /// </summary>
    public static class SystemAdministration
    {
        public const string ViewAuditLogs = "Permissions.SystemAdministration.ViewAuditLogs";
        public const string ManageSystemSettings = "Permissions.SystemAdministration.ManageSystemSettings";
        public const string ViewHealthChecks = "Permissions.SystemAdministration.ViewHealthChecks";
    }
}

