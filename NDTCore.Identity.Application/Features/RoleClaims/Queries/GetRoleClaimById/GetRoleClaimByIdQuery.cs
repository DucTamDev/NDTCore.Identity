using NDTCore.Identity.Application.Common.Interfaces;
using NDTCore.Identity.Contracts.Features.Claims.DTOs;

namespace NDTCore.Identity.Application.Features.RoleClaims.Queries.GetRoleClaimById;

/// <summary>
/// Query to get a specific role claim by ID
/// </summary>
public record GetRoleClaimByIdQuery : IQuery<RoleClaimDto>
{
    public int ClaimId { get; init; }
}

