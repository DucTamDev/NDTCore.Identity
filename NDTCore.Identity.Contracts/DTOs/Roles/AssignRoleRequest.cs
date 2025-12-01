using System.ComponentModel.DataAnnotations;

namespace NDTCore.Identity.Contracts.DTOs.Roles;

public class AssignRoleRequest
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid RoleId { get; set; }
}
