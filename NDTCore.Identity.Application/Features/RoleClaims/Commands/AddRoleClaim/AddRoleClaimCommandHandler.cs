using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Common.Results;
using NDTCore.Identity.Contracts.Features.Claims.DTOs;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Domain.Constants;
using NDTCore.Identity.Domain.Entities;
using NDTCore.Identity.Domain.Exceptions;

namespace NDTCore.Identity.Application.Features.RoleClaims.Commands.AddRoleClaim;

/// <summary>
/// Handler for adding a claim to a role
/// </summary>
public class AddRoleClaimCommandHandler : IRequestHandler<AddRoleClaimCommand, Result<RoleClaimDto>>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IRoleClaimRepository _roleClaimRepository;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly ILogger<AddRoleClaimCommandHandler> _logger;

    public AddRoleClaimCommandHandler(
        IRoleRepository roleRepository,
        IRoleClaimRepository roleClaimRepository,
        RoleManager<AppRole> roleManager,
        ILogger<AddRoleClaimCommandHandler> logger)
    {
        _roleRepository = roleRepository;
        _roleClaimRepository = roleClaimRepository;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task<Result<RoleClaimDto>> Handle(AddRoleClaimCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
            if (role == null)
                return Result<RoleClaimDto>.NotFound($"Role with ID '{request.RoleId}' was not found");

            // Use RoleManager to add claim
            var claim = new System.Security.Claims.Claim(request.ClaimType, request.ClaimValue);
            var addResult = await _roleManager.AddClaimAsync(role, claim);

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

            // Get the created claim
            var createdClaim = await _roleClaimRepository.GetRoleClaimAsync(
                request.RoleId,
                request.ClaimType,
                request.ClaimValue,
                cancellationToken);

            if (createdClaim != null)
            {
                var dto = MapToRoleClaimDto(createdClaim);
                _logger.LogInformation("Successfully added claim {ClaimType} to role {RoleId}", request.ClaimType, request.RoleId);
                return Result<RoleClaimDto>.Created(dto, "Claim added successfully");
            }

            // Fallback DTO
            var fallbackDto = new RoleClaimDto
            {
                RoleId = role.Id,
                RoleName = role.Name ?? string.Empty,
                ClaimType = request.ClaimType,
                ClaimValue = request.ClaimValue
            };

            return Result<RoleClaimDto>.Created(fallbackDto, "Claim added successfully");
        }
        catch (ConflictException ex)
        {
            _logger.LogWarning(ex, "Conflict adding claim to role {RoleId}", request.RoleId);
            return Result<RoleClaimDto>.Conflict(ex.Message);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain error adding claim to role {RoleId}", request.RoleId);
            return Result<RoleClaimDto>.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding role claim to role {RoleId}", request.RoleId);
            return Result<RoleClaimDto>.InternalError("An error occurred while adding the claim");
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

