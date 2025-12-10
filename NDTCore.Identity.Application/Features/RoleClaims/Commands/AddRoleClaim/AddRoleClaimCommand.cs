using NDTCore.Identity.Application.Common.Interfaces;
using NDTCore.Identity.Contracts.Features.Claims.DTOs;

namespace NDTCore.Identity.Application.Features.RoleClaims.Commands.AddRoleClaim;

/// <summary>
/// Command to add a claim to a role
/// </summary>
public record AddRoleClaimCommand : ICommand<RoleClaimDto>
{
    public Guid RoleId { get; init; }
    public string ClaimType { get; init; } = string.Empty;
    public string ClaimValue { get; init; } = string.Empty;
}

