using NDTCore.Identity.Application.Common.Interfaces;
using NDTCore.Identity.Contracts.Features.Claims.DTOs;

namespace NDTCore.Identity.Application.Features.UserClaims.Commands.AddUserClaim;

/// <summary>
/// Command to add a claim to a user
/// </summary>
public record AddUserClaimCommand : ICommand<UserClaimDto>
{
    public Guid UserId { get; init; }
    public string ClaimType { get; init; } = string.Empty;
    public string ClaimValue { get; init; } = string.Empty;
}

