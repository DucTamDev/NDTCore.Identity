using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Common.Results;
using NDTCore.Identity.Contracts.Features.Claims.DTOs;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Domain.Constants;
using NDTCore.Identity.Domain.Entities;
using NDTCore.Identity.Domain.Exceptions;

namespace NDTCore.Identity.Application.Features.RoleClaims.Commands.UpdateRoleClaim;

/// <summary>
/// Handler for updating a role claim
/// </summary>
public class UpdateRoleClaimCommandHandler : IRequestHandler<UpdateRoleClaimCommand, Result<RoleClaimDto>>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IRoleClaimRepository _roleClaimRepository;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly ILogger<UpdateRoleClaimCommandHandler> _logger;

    public UpdateRoleClaimCommandHandler(
        IRoleRepository roleRepository,
        IRoleClaimRepository roleClaimRepository,
        RoleManager<AppRole> roleManager,
        ILogger<UpdateRoleClaimCommandHandler> logger)
    {
        _roleRepository = roleRepository;
        _roleClaimRepository = roleClaimRepository;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task<Result<RoleClaimDto>> Handle(UpdateRoleClaimCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var oldClaim = await _roleClaimRepository.GetByIdAsync(request.ClaimId, cancellationToken);
            if (oldClaim == null)
                return Result<RoleClaimDto>.NotFound($"Role claim with ID '{request.ClaimId}' was not found");

            var role = await _roleRepository.GetByIdAsync(oldClaim.RoleId, cancellationToken);
            if (role == null)
                return Result<RoleClaimDto>.NotFound($"Role with ID '{oldClaim.RoleId}' was not found");

            // Remove old claim and add new claim
            var oldClaimObj = new System.Security.Claims.Claim(oldClaim.ClaimType!, oldClaim.ClaimValue!);
            var removeResult = await _roleManager.RemoveClaimAsync(role, oldClaimObj);

            if (!removeResult.Succeeded)
            {
                var validationErrors = removeResult.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.Description).ToList());

                return Result<RoleClaimDto>.BadRequest(
                    message: "One or more validation errors occurred",
                    errorCode: ErrorCodes.ValidationError,
                    validationErrors: validationErrors);
            }

            var newClaim = new System.Security.Claims.Claim(request.ClaimType, request.ClaimValue);
            var addResult = await _roleManager.AddClaimAsync(role, newClaim);

            if (!addResult.Succeeded)
            {
                var validationErrors = addResult.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.Description).ToList());

                return Result<RoleClaimDto>.BadRequest(
                    message: "One or more validation errors occurred",
                    errorCode: ErrorCodes.ValidationError,
                    validationErrors: validationErrors);
            }

            // Get updated claim
            var updatedClaim = await _roleClaimRepository.GetRoleClaimAsync(
                role.Id,
                request.ClaimType,
                request.ClaimValue,
                cancellationToken);

            if (updatedClaim != null)
            {
                var dto = MapToRoleClaimDto(updatedClaim);
                _logger.LogInformation("Successfully updated claim {ClaimId} for role {RoleId}", request.ClaimId, role.Id);
                return Result<RoleClaimDto>.Success(dto, "Claim updated successfully");
            }

            return Result<RoleClaimDto>.InternalError("Failed to retrieve updated claim");
        }
        catch (ConflictException ex)
        {
            _logger.LogWarning(ex, "Conflict updating claim {ClaimId}", request.ClaimId);
            return Result<RoleClaimDto>.Conflict(ex.Message);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain error updating claim {ClaimId}", request.ClaimId);
            return Result<RoleClaimDto>.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating role claim {ClaimId}", request.ClaimId);
            return Result<RoleClaimDto>.InternalError("An error occurred while updating the claim");
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

