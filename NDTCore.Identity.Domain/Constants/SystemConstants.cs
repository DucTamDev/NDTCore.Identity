namespace NDTCore.Identity.Domain.Constants;

/// <summary>
/// System-wide constants to avoid hard-coding values
/// </summary>
public static class SystemConstants
{
    /// <summary>
    /// Default role names
    /// </summary>
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string User = "User";
        public const string Manager = "Manager";
    }

    /// <summary>
    /// System operation identifiers
    /// </summary>
    public static class SystemOperations
    {
        public const string TokenLimitRevocation = "System - Token limit";
        public const string Registration = "Registration";
        public const string System = "System";
        public const string Logout = "Logout";
    }

    /// <summary>
    /// Entity type names for audit logging
    /// </summary>
    public static class EntityTypes
    {
        public const string User = "User";
        public const string Role = "Role";
        public const string Permission = "Permission";
        public const string UserRole = "UserRole";
        public const string UserClaim = "UserClaim";
        public const string RoleClaim = "RoleClaim";
        public const string RefreshToken = "RefreshToken";
    }

    /// <summary>
    /// Audit action names
    /// </summary>
    public static class AuditActions
    {
        public const string Create = "Create";
        public const string Update = "Update";
        public const string Delete = "Delete";
        public const string View = "View";
        public const string Login = "Login";
        public const string Logout = "Logout";
    }

    /// <summary>
    /// Token management constants
    /// </summary>
    public static class TokenManagement
    {
        /// <summary>
        /// Default maximum active tokens per user (can be overridden by config)
        /// </summary>
        public const int DefaultMaxActiveTokensPerUser = 5;

        /// <summary>
        /// Default number of tokens to keep when revoking (max - 1)
        /// </summary>
        public const int DefaultTokensToKeep = 4;
    }
}

