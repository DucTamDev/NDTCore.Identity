using NDTCore.Identity.Application.Common.Interfaces;

namespace NDTCore.Identity.Application.Features.UserClaims.Commands.RemoveUserClaim;

/// <summary>
/// Command to remove a claim from a user
/// </summary>
public record RemoveUserClaimCommand : ICommand
{
    public int ClaimId { get; init; }
}

