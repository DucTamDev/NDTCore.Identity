using NDTCore.Identity.Application.Common.Interfaces;

namespace NDTCore.Identity.Application.Features.Users.Commands.DeleteUser;

public record DeleteUserCommand : ICommand
{
    public Guid UserId { get; init; }
}

