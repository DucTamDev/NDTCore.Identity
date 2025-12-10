using NDTCore.Identity.Application.Common.Interfaces;
using NDTCore.Identity.Contracts.Features.Claims.DTOs;

namespace NDTCore.Identity.Application.Features.RoleClaims.Queries.GetRoleClaims;

/// <summary>
/// Query to get all claims for a specific role
/// </summary>
public record GetRoleClaimsQuery : IQuery<List<RoleClaimDto>>
{
    public Guid RoleId { get; init; }
}

