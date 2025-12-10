using NDTCore.Identity.Application.Common.Interfaces;
using NDTCore.Identity.Contracts.Features.Users.DTOs;

namespace NDTCore.Identity.Application.Features.Users.Queries.GetUserById;

public record GetUserByIdQuery : IQuery<UserDto>
{
    public Guid UserId { get; init; }
}

