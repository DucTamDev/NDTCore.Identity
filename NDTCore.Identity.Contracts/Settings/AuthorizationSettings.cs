namespace NDTCore.Identity.Contracts.Settings;

public class AuthorizationSettings
{
    public const string SectionName = "AuthorizationSettings";

    public PermissionSettings Permissions { get; set; } = new();
}
