using NDTCore.Identity.Application.Common.Interfaces;
using NDTCore.Identity.Contracts.Features.Roles.DTOs;

namespace NDTCore.Identity.Application.Features.Roles.Queries.GetRoleById;

public record GetRoleByIdQuery : IQuery<RoleDto>
{
    public Guid RoleId { get; init; }
}

