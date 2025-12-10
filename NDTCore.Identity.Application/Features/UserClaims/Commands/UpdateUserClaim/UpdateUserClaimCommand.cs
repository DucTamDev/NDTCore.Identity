using NDTCore.Identity.Application.Common.Interfaces;
using NDTCore.Identity.Contracts.Features.Claims.DTOs;

namespace NDTCore.Identity.Application.Features.UserClaims.Commands.UpdateUserClaim;

/// <summary>
/// Command to update a user claim
/// </summary>
public record UpdateUserClaimCommand : ICommand<UserClaimDto>
{
    public int ClaimId { get; init; }
    public string ClaimType { get; init; } = string.Empty;
    public string ClaimValue { get; init; } = string.Empty;
}

