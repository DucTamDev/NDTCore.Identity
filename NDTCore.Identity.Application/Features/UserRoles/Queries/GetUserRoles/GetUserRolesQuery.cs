using NDTCore.Identity.Application.Common.Interfaces;
using NDTCore.Identity.Contracts.Common.Pagination;
using NDTCore.Identity.Contracts.Features.UserRoles.DTOs;

namespace NDTCore.Identity.Application.Features.UserRoles.Queries.GetUserRoles;

/// <summary>
/// Query to get paginated list of user-role assignments
/// </summary>
public record GetUserRolesQuery : IQuery<PaginatedCollection<UserRoleDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public Guid? UserId { get; init; }
    public Guid? RoleId { get; init; }
}

