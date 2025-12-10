using NDTCore.Identity.Application.Common.Interfaces;

namespace NDTCore.Identity.Application.Features.RoleClaims.Commands.RemoveRoleClaim;

/// <summary>
/// Command to remove a claim from a role
/// </summary>
public record RemoveRoleClaimCommand : ICommand
{
    public int ClaimId { get; init; }
}

