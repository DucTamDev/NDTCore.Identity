using NDTCore.Identity.Application.Common.Interfaces;
using NDTCore.Identity.Contracts.Features.Claims.DTOs;

namespace NDTCore.Identity.Application.Features.RoleClaims.Commands.UpdateRoleClaim;

/// <summary>
/// Command to update a role claim
/// </summary>
public record UpdateRoleClaimCommand : ICommand<RoleClaimDto>
{
    public int ClaimId { get; init; }
    public string ClaimType { get; init; } = string.Empty;
    public string ClaimValue { get; init; } = string.Empty;
}

