using NDTCore.Identity.Application.Common.Interfaces;

namespace NDTCore.Identity.Application.Features.Roles.Commands.DeleteRole;

public record DeleteRoleCommand : ICommand
{
    public Guid RoleId { get; init; }
}

