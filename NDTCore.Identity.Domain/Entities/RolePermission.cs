using NDTCore.Identity.Domain.Common;

namespace NDTCore.Identity.Domain.Entities;

public class RolePermission : BaseEntity
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public string? AssignedBy { get; set; }

    // Navigation
    public AppRole Role { get; set; } = default!;
    public Permission Permission { get; set; } = default!;
}


