using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Common.Results;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Domain.Constants;
using NDTCore.Identity.Domain.Entities;
using NDTCore.Identity.Domain.Exceptions;

namespace NDTCore.Identity.Application.Features.UserClaims.Commands.RemoveUserClaim;

/// <summary>
/// Handler for removing a claim from a user
/// </summary>
public class RemoveUserClaimCommandHandler : IRequestHandler<RemoveUserClaimCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserClaimRepository _userClaimRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<RemoveUserClaimCommandHandler> _logger;

    public RemoveUserClaimCommandHandler(
        IUserRepository userRepository,
        IUserClaimRepository userClaimRepository,
        UserManager<AppUser> userManager,
        ILogger<RemoveUserClaimCommandHandler> logger)
    {
        _userRepository = userRepository;
        _userClaimRepository = userClaimRepository;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result> Handle(RemoveUserClaimCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var claim = await _userClaimRepository.GetByIdAsync(request.ClaimId, cancellationToken);
            if (claim == null)
                return Result.NotFound($"User claim with ID '{request.ClaimId}' was not found");

            var user = await _userRepository.GetByIdAsync(claim.UserId, cancellationToken);
            if (user == null)
                return Result.NotFound($"User with ID '{claim.UserId}' was not found");

            var claimObj = new System.Security.Claims.Claim(claim.ClaimType!, claim.ClaimValue!);
            var removeResult = await _userManager.RemoveClaimAsync(user, claimObj);

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

            _logger.LogInformation("Successfully removed claim {ClaimId} from user {UserId}", request.ClaimId, claim.UserId);
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
            _logger.LogError(ex, "Error removing user claim {ClaimId}", request.ClaimId);
            return Result.InternalError("An error occurred while removing the claim");
        }
    }
}

