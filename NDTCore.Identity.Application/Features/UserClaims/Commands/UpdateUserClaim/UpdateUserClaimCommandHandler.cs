using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Common.Results;
using NDTCore.Identity.Contracts.Features.Claims.DTOs;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Domain.Constants;
using NDTCore.Identity.Domain.Entities;
using NDTCore.Identity.Domain.Exceptions;

namespace NDTCore.Identity.Application.Features.UserClaims.Commands.UpdateUserClaim;

/// <summary>
/// Handler for updating a user claim
/// </summary>
public class UpdateUserClaimCommandHandler : IRequestHandler<UpdateUserClaimCommand, Result<UserClaimDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserClaimRepository _userClaimRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<UpdateUserClaimCommandHandler> _logger;

    public UpdateUserClaimCommandHandler(
        IUserRepository userRepository,
        IUserClaimRepository userClaimRepository,
        UserManager<AppUser> userManager,
        ILogger<UpdateUserClaimCommandHandler> logger)
    {
        _userRepository = userRepository;
        _userClaimRepository = userClaimRepository;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result<UserClaimDto>> Handle(UpdateUserClaimCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var oldClaim = await _userClaimRepository.GetByIdAsync(request.ClaimId, cancellationToken);
            if (oldClaim == null)
                return Result<UserClaimDto>.NotFound($"User claim with ID '{request.ClaimId}' was not found");

            var user = await _userRepository.GetByIdAsync(oldClaim.UserId, cancellationToken);
            if (user == null)
                return Result<UserClaimDto>.NotFound($"User with ID '{oldClaim.UserId}' was not found");

            // Remove old claim and add new claim
            var oldClaimObj = new System.Security.Claims.Claim(oldClaim.ClaimType!, oldClaim.ClaimValue!);
            var removeResult = await _userManager.RemoveClaimAsync(user, oldClaimObj);

            if (!removeResult.Succeeded)
            {
                var validationErrors = removeResult.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.Description).ToList());

                return Result<UserClaimDto>.BadRequest(
                    message: "One or more validation errors occurred",
                    errorCode: ErrorCodes.ValidationError,
                    validationErrors: validationErrors);
            }

            var newClaim = new System.Security.Claims.Claim(request.ClaimType, request.ClaimValue);
            var addResult = await _userManager.AddClaimAsync(user, newClaim);

            if (!addResult.Succeeded)
            {
                var validationErrors = addResult.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.Description).ToList());

                return Result<UserClaimDto>.BadRequest(
                    message: "One or more validation errors occurred",
                    errorCode: ErrorCodes.ValidationError,
                    validationErrors: validationErrors);
            }

            // Get updated claim
            var updatedClaim = await _userClaimRepository.GetUserClaimAsync(
                user.Id,
                request.ClaimType,
                request.ClaimValue,
                cancellationToken);

            if (updatedClaim != null)
            {
                var dto = MapToUserClaimDto(updatedClaim);
                _logger.LogInformation("Successfully updated claim {ClaimId} for user {UserId}", request.ClaimId, user.Id);
                return Result<UserClaimDto>.Success(dto, "Claim updated successfully");
            }

            return Result<UserClaimDto>.InternalError("Failed to retrieve updated claim");
        }
        catch (ConflictException ex)
        {
            _logger.LogWarning(ex, "Conflict updating claim {ClaimId}", request.ClaimId);
            return Result<UserClaimDto>.Conflict(ex.Message);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain error updating claim {ClaimId}", request.ClaimId);
            return Result<UserClaimDto>.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user claim {ClaimId}", request.ClaimId);
            return Result<UserClaimDto>.InternalError("An error occurred while updating the claim");
        }
    }

    private static UserClaimDto MapToUserClaimDto(AppUserClaim claim)
    {
        return new UserClaimDto
        {
            Id = claim.Id,
            UserId = claim.UserId,
            UserName = claim.AppUser?.UserName ?? string.Empty,
            ClaimType = claim.ClaimType ?? string.Empty,
            ClaimValue = claim.ClaimValue ?? string.Empty
        };
    }
}

