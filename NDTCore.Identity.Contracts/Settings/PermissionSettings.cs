namespace NDTCore.Identity.Contracts.Settings;


/// <summary>
/// Configuration options for permissions loaded from appsettings
/// </summary>
public class PermissionSettings
{
    public List<PermissionModuleConfig> Modules { get; set; } = new();
}

public class PermissionModuleConfig
{
    public string Name { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
    public int SortOrder { get; set; }
    public List<PermissionConfig> Permissions { get; set; } = new();
}

public class PermissionConfig
{
    public string Name { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
    public string? Group { get; set; }
    public int SortOrder { get; set; }
}
