using NDTCore.Identity.Application.Common.Interfaces;
using NDTCore.Identity.Contracts.Features.UserRoles.DTOs;

namespace NDTCore.Identity.Application.Features.UserRoles.Queries.GetUserRolesByRoleId;

/// <summary>
/// Query to get all users assigned to a role
/// </summary>
public record GetUserRolesByRoleIdQuery : IQuery<List<UserRoleDto>>
{
    public Guid RoleId { get; init; }
}

