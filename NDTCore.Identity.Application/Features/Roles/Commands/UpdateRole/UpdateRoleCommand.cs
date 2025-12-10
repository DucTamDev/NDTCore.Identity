using NDTCore.Identity.Application.Common.Interfaces;

namespace NDTCore.Identity.Application.Features.Roles.Commands.UpdateRole;

public record UpdateRoleCommand : ICommand
{
    public Guid RoleId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
}

