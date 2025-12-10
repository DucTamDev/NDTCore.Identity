using MediatR;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Common.Results;
using NDTCore.Identity.Contracts.Features.Claims.DTOs;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Application.Features.RoleClaims.Queries.GetRoleClaimById;

/// <summary>
/// Handler for retrieving a specific role claim by ID
/// </summary>
public class GetRoleClaimByIdQueryHandler : IRequestHandler<GetRoleClaimByIdQuery, Result<RoleClaimDto>>
{
    private readonly IRoleClaimRepository _roleClaimRepository;
    private readonly ILogger<GetRoleClaimByIdQueryHandler> _logger;

    public GetRoleClaimByIdQueryHandler(
        IRoleClaimRepository roleClaimRepository,
        ILogger<GetRoleClaimByIdQueryHandler> logger)
    {
        _roleClaimRepository = roleClaimRepository;
        _logger = logger;
    }

    public async Task<Result<RoleClaimDto>> Handle(GetRoleClaimByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var claim = await _roleClaimRepository.GetByIdAsync(request.ClaimId, cancellationToken);
            if (claim == null)
                return Result<RoleClaimDto>.NotFound($"Role claim with ID '{request.ClaimId}' was not found");

            var dto = MapToRoleClaimDto(claim);
            _logger.LogInformation("Retrieved role claim {ClaimId}", request.ClaimId);
            return Result<RoleClaimDto>.Success(dto, "Role claim retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting role claim {ClaimId}", request.ClaimId);
            return Result<RoleClaimDto>.InternalError("An error occurred while retrieving the role claim");
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

