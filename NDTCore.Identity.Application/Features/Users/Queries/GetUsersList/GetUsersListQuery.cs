using NDTCore.Identity.Application.Common.Interfaces;
using NDTCore.Identity.Contracts.Common.Pagination;
using NDTCore.Identity.Contracts.Features.Users.DTOs;

namespace NDTCore.Identity.Application.Features.Users.Queries.GetUsersList;

public record GetUsersListQuery : IQuery<PaginatedCollection<UserDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? SearchTerm { get; init; }
}

