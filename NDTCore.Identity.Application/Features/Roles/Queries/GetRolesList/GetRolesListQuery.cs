using NDTCore.Identity.Application.Common.Interfaces;
using NDTCore.Identity.Contracts.Common.Pagination;
using NDTCore.Identity.Contracts.Features.Roles.DTOs;

namespace NDTCore.Identity.Application.Features.Roles.Queries.GetRolesList;

public record GetRolesListQuery : IQuery<PaginatedCollection<RoleDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? SearchTerm { get; init; }
}

