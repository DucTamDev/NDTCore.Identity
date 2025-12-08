namespace NDTCore.Identity.Contracts.Configuration.Authorization;

public class AuthorizationSettings
{
    public const string SectionName = "AuthorizationSettings";

    public PermissionOptions Permissions { get; set; } = new();
}
