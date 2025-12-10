using NDTCore.Identity.Application.Common.Interfaces;

namespace NDTCore.Identity.Application.Features.UserRoles.Commands.RemoveRoleFromUser;

/// <summary>
/// Command to remove a role from a user
/// </summary>
public record RemoveRoleFromUserCommand : ICommand
{
    public Guid UserId { get; init; }
    public Guid RoleId { get; init; }
}

