using NDTCore.Identity.Application.Common.Interfaces;
using NDTCore.Identity.Contracts.Features.UserRoles.DTOs;

namespace NDTCore.Identity.Application.Features.UserRoles.Queries.GetUserRolesByUserId;

/// <summary>
/// Query to get all roles assigned to a user
/// </summary>
public record GetUserRolesByUserIdQuery : IQuery<List<UserRoleDto>>
{
    public Guid UserId { get; init; }
}

