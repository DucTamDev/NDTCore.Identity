namespace NDTCore.Identity.Contracts.Authorization.Permissions;

/// <summary>
/// Represents a module/category of permissions
/// </summary>
public sealed class PermissionModule
{
    public string Name { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public int SortOrder { get; private set; }
    public List<IPermissionDefinition> Permissions { get; private set; } = new();

    private PermissionModule() { }

    /// <summary>
    /// Creates a new module builder
    /// </summary>
    public static ModuleBuilder Create(string name) => new(name);

    /// <summary>
    /// Fluent builder for creating permission modules
    /// </summary>
    public class ModuleBuilder
    {
        private readonly PermissionModule _module;

        internal ModuleBuilder(string name)
        {
            _module = new PermissionModule { Name = name };
        }

        public ModuleBuilder WithDisplayName(string displayName)
        {
            _module.DisplayName = displayName;
            return this;
        }

        public ModuleBuilder WithDescription(string description)
        {
            _module.Description = description;
            return this;
        }

        public ModuleBuilder WithSortOrder(int sortOrder)
        {
            _module.SortOrder = sortOrder;
            return this;
        }

        public ModuleBuilder WithPermissions(params IPermissionDefinition[] permissions)
        {
            _module.Permissions.AddRange(permissions);
            return this;
        }

        public PermissionModule Build() => _module;
    }

    public override string ToString() => Name;
}
