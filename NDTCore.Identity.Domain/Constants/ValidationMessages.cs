namespace NDTCore.Identity.Domain.Constants;

/// <summary>
/// Validation message constants
/// </summary>
public static class ValidationMessages
{
    // User validation messages
    public const string UserEmailRequired = "Email is required";
    public const string UserEmailInvalid = "Email format is invalid";
    public const string UserFirstNameRequired = "First name is required";
    public const string UserLastNameRequired = "Last name is required";
    public const string UserPasswordRequired = "Password is required";
    public const string UserPasswordTooShort = "Password must be at least 6 characters";

    // Role validation messages
    public const string RoleNameRequired = "Role name is required";
    public const string RoleNameInvalid = "Role name contains invalid characters";

    // Permission validation messages
    public const string PermissionNameRequired = "Permission name is required";
    public const string PermissionModuleRequired = "Permission module is required";

    // Common validation messages
    public const string IdRequired = "Id is required";
    public const string IdInvalid = "Id format is invalid";
}

