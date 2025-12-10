using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Common.Results;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Domain.Constants;
using NDTCore.Identity.Domain.Entities;
using NDTCore.Identity.Domain.Exceptions;

namespace NDTCore.Identity.Application.Features.RoleClaims.Commands.RemoveRoleClaim;

/// <summary>
/// Handler for removing a claim from a role
/// </summary>
public class RemoveRoleClaimCommandHandler : IRequestHandler<RemoveRoleClaimCommand, Result>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IRoleClaimRepository _roleClaimRepository;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly ILogger<RemoveRoleClaimCommandHandler> _logger;

    public RemoveRoleClaimCommandHandler(
        IRoleRepository roleRepository,
        IRoleClaimRepository roleClaimRepository,
        RoleManager<AppRole> roleManager,
        ILogger<RemoveRoleClaimCommandHandler> logger)
    {
        _roleRepository = roleRepository;
        _roleClaimRepository = roleClaimRepository;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task<Result> Handle(RemoveRoleClaimCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var claim = await _roleClaimRepository.GetByIdAsync(request.ClaimId, cancellationToken);
            if (claim == null)
                return Result.NotFound($"Role claim with ID '{request.ClaimId}' was not found");

            var role = await _roleRepository.GetByIdAsync(claim.RoleId, cancellationToken);
            if (role == null)
                return Result.NotFound($"Role with ID '{claim.RoleId}' was not found");

            var claimObj = new System.Security.Claims.Claim(claim.ClaimType!, claim.ClaimValue!);
            var removeResult = await _roleManager.RemoveClaimAsync(role, claimObj);

            if (!removeResult.Succeeded)
            {
                var validationErrors = removeResult.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.Description).ToList());

                return Result.BadRequest(
                    message: "One or more validation errors occurred",
                    errorCode: ErrorCodes.ValidationError,
                    validationErrors: validationErrors);
            }

            _logger.LogInformation("Successfully removed claim {ClaimId} from role {RoleId}", request.ClaimId, claim.RoleId);
            return Result.Success("Claim removed successfully");
        }
        catch (ConflictException ex)
        {
            _logger.LogWarning(ex, "Conflict removing claim {ClaimId}", request.ClaimId);
            return Result.Conflict(ex.Message);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain error removing claim {ClaimId}", request.ClaimId);
            return Result.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing role claim {ClaimId}", request.ClaimId);
            return Result.InternalError("An error occurred while removing the claim");
        }
    }
}

