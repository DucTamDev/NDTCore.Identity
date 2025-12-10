using NDTCore.Identity.Application.Common.Interfaces;
using NDTCore.Identity.Contracts.Features.UserRoles.DTOs;

namespace NDTCore.Identity.Application.Features.UserRoles.Queries.GetUserRole;

/// <summary>
/// Query to get a specific user-role assignment
/// </summary>
public record GetUserRoleQuery : IQuery<UserRoleDto>
{
    public Guid UserId { get; init; }
    public Guid RoleId { get; init; }
}

