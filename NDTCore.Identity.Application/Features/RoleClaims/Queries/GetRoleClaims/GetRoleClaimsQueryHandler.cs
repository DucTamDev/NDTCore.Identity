using MediatR;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Common.Results;
using NDTCore.Identity.Contracts.Features.Claims.DTOs;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Application.Features.RoleClaims.Queries.GetRoleClaims;

/// <summary>
/// Handler for retrieving all claims for a specific role
/// </summary>
public class GetRoleClaimsQueryHandler : IRequestHandler<GetRoleClaimsQuery, Result<List<RoleClaimDto>>>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IRoleClaimRepository _roleClaimRepository;
    private readonly ILogger<GetRoleClaimsQueryHandler> _logger;

    public GetRoleClaimsQueryHandler(
        IRoleRepository roleRepository,
        IRoleClaimRepository roleClaimRepository,
        ILogger<GetRoleClaimsQueryHandler> logger)
    {
        _roleRepository = roleRepository;
        _roleClaimRepository = roleClaimRepository;
        _logger = logger;
    }

    public async Task<Result<List<RoleClaimDto>>> Handle(GetRoleClaimsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
            if (role == null)
                return Result<List<RoleClaimDto>>.NotFound($"Role with ID '{request.RoleId}' was not found");

            var claims = await _roleClaimRepository.GetClaimsByRoleIdAsync(request.RoleId, cancellationToken);
            var dtos = claims.Select(MapToRoleClaimDto).ToList();

            _logger.LogInformation("Retrieved {Count} claims for role {RoleId}", dtos.Count, request.RoleId);
            return Result<List<RoleClaimDto>>.Success(dtos, "Role claims retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting role claims for role {RoleId}", request.RoleId);
            return Result<List<RoleClaimDto>>.InternalError("An error occurred while retrieving role claims");
        }
    }

    private static RoleClaimDto MapToRoleClaimDto(AppRoleClaim claim)
    {
        return new RoleClaimDto
        {
            Id = claim.Id,
            RoleId = claim.RoleId,
            RoleName = claim.AppRole?.Name ?? string.Empty,
            ClaimType = claim.ClaimType ?? string.Empty,
            ClaimValue = claim.ClaimValue ?? string.Empty
        };
    }
}

