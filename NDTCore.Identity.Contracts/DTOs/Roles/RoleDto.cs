namespace NDTCore.Identity.Contracts.DTOs.Roles;

public class RoleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Priority { get; set; }
    public bool IsSystemRole { get; set; }
    public DateTime CreatedAt { get; set; }
}
