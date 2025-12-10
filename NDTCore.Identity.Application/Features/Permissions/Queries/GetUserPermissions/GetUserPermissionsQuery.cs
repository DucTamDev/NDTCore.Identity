using NDTCore.Identity.Application.Common.Interfaces;
using NDTCore.Identity.Contracts.Features.Permissions.DTOs;

namespace NDTCore.Identity.Application.Features.Permissions.Queries.GetUserPermissions;

public record GetUserPermissionsQuery : IQuery<UserPermissionsDto>
{
    public Guid UserId { get; init; }
}

