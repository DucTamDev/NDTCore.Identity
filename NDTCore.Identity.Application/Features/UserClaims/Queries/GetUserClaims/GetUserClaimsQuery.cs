using NDTCore.Identity.Application.Common.Interfaces;
using NDTCore.Identity.Contracts.Features.Claims.DTOs;

namespace NDTCore.Identity.Application.Features.UserClaims.Queries.GetUserClaims;

/// <summary>
/// Query to get all claims for a specific user
/// </summary>
public record GetUserClaimsQuery : IQuery<List<UserClaimDto>>
{
    public Guid UserId { get; init; }
}

