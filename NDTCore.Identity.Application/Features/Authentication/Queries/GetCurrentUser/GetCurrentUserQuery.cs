using NDTCore.Identity.Application.Common.Interfaces;
using NDTCore.Identity.Contracts.Features.Users.DTOs;

namespace NDTCore.Identity.Application.Features.Authentication.Queries.GetCurrentUser;

/// <summary>
/// Query to get current authenticated user
/// </summary>
public record GetCurrentUserQuery : IQuery<UserDto>
{
    public Guid UserId { get; init; }
}

