using NDTCore.Identity.Application.Common.Interfaces;
using NDTCore.Identity.Contracts.Features.Permissions.DTOs;

namespace NDTCore.Identity.Application.Features.Permissions.Queries.GetAllPermissions;

public record GetAllPermissionsQuery : IQuery<List<PermissionDto>>
{
    public bool GroupByModule { get; init; }
}

