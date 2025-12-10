using NDTCore.Identity.Application.Common.Interfaces;
using NDTCore.Identity.Contracts.Features.Claims.DTOs;

namespace NDTCore.Identity.Application.Features.UserClaims.Queries.GetUserClaimById;

/// <summary>
/// Query to get a specific user claim by ID
/// </summary>
public record GetUserClaimByIdQuery : IQuery<UserClaimDto>
{
    public int ClaimId { get; init; }
}

