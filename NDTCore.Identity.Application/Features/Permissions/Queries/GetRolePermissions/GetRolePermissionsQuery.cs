using NDTCore.Identity.Application.Common.Interfaces;

namespace NDTCore.Identity.Application.Features.Permissions.Queries.GetRolePermissions;

public record GetRolePermissionsQuery : IQuery<List<string>>
{
    public Guid RoleId { get; init; }
}

