namespace NDTCore.Identity.Contracts.Authorization.Permissions;

/// <summary>
/// Concrete implementation of a permission definition
/// </summary>
public sealed class PermissionDefinition : IPermissionDefinition
{
    public string Name { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public string Module { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsSystemPermission { get; private set; } = true;
    public string? Group { get; private set; }

    private PermissionDefinition() { }

    /// <summary>
    /// Creates a new permission builder
    /// </summary>
    public static PermissionBuilder Create(string name) => new(name);

    /// <summary>
    /// Fluent builder for creating permission definitions
    /// </summary>
    public class PermissionBuilder
    {
        private readonly PermissionDefinition _permission;

        internal PermissionBuilder(string name)
        {
            _permission = new PermissionDefinition { Name = name };
        }

        public PermissionBuilder WithDisplayName(string displayName)
        {
            _permission.DisplayName = displayName;
            return this;
        }

        public PermissionBuilder InModule(string module)
        {
            _permission.Module = module;
            return this;
        }

        public PermissionBuilder WithDescription(string description)
        {
            _permission.Description = description;
            return this;
        }

        public PermissionBuilder WithSortOrder(int sortOrder)
        {
            _permission.SortOrder = sortOrder;
            return this;
        }

        public PermissionBuilder AsCustomPermission()
        {
            _permission.IsSystemPermission = false;
            return this;
        }

        public PermissionBuilder InGroup(string group)
        {
            _permission.Group = group;
            return this;
        }

        public PermissionDefinition Build() => _permission;
    }

    public override string ToString() => Name;

    public override bool Equals(object? obj) => obj is PermissionDefinition other && Name == other.Name;

    public override int GetHashCode() => Name.GetHashCode();
}
