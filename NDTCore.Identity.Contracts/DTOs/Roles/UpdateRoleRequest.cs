using System.ComponentModel.DataAnnotations;

namespace NDTCore.Identity.Contracts.DTOs.Roles;

public class UpdateRoleRequest
{
    [Required(ErrorMessage = "Role name is required")]
    [StringLength(50, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public int Priority { get; set; }
}