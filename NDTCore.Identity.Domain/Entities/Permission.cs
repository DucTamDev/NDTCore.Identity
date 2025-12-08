using NDTCore.Identity.Domain.BaseEntities;

namespace NDTCore.Identity.Domain.Entities;

public class Permission : BaseEntity, IAggregateRoot
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsSystemPermission { get; set; }
    public int SortOrder { get; set; }

    // Navigation
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}


