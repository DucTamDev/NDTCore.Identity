using NDTCore.Identity.Application.Common.Interfaces;

namespace NDTCore.Identity.Application.Features.Roles.Commands.CreateRole;

public record CreateRoleCommand : ICommand<Guid>
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
}

