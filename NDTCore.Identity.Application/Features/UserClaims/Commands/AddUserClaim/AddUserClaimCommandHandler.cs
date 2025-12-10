using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Common.Results;
using NDTCore.Identity.Contracts.Features.Claims.DTOs;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Domain.Constants;
using NDTCore.Identity.Domain.Entities;
using NDTCore.Identity.Domain.Exceptions;

namespace NDTCore.Identity.Application.Features.UserClaims.Commands.AddUserClaim;

/// <summary>
/// Handler for adding a claim to a user
/// </summary>
public class AddUserClaimCommandHandler : IRequestHandler<AddUserClaimCommand, Result<UserClaimDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserClaimRepository _userClaimRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<AddUserClaimCommandHandler> _logger;

    public AddUserClaimCommandHandler(
        IUserRepository userRepository,
        IUserClaimRepository userClaimRepository,
        UserManager<AppUser> userManager,
        ILogger<AddUserClaimCommandHandler> logger)
    {
        _userRepository = userRepository;
        _userClaimRepository = userClaimRepository;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result<UserClaimDto>> Handle(AddUserClaimCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
                return Result<UserClaimDto>.NotFound($"User with ID '{request.UserId}' was not found");

            // Use UserManager to add claim (for Identity framework consistency)
            var claim = new System.Security.Claims.Claim(request.ClaimType, request.ClaimValue);
            var addResult = await _userManager.AddClaimAsync(user, claim);

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

            // Get the created claim
            var createdClaim = await _userClaimRepository.GetUserClaimAsync(
                request.UserId,
                request.ClaimType,
                request.ClaimValue,
                cancellationToken);

            if (createdClaim != null)
            {
                var dto = MapToUserClaimDto(createdClaim);
                _logger.LogInformation("Successfully added claim {ClaimType} to user {UserId}", request.ClaimType, request.UserId);
                return Result<UserClaimDto>.Created(dto, "Claim added successfully");
            }

            // Fallback DTO
            var fallbackDto = new UserClaimDto
            {
                UserId = user.Id,
                UserName = user.UserName ?? string.Empty,
                ClaimType = request.ClaimType,
                ClaimValue = request.ClaimValue
            };

            return Result<UserClaimDto>.Created(fallbackDto, "Claim added successfully");
        }
        catch (ConflictException ex)
        {
            _logger.LogWarning(ex, "Conflict adding claim to user {UserId}", request.UserId);
            return Result<UserClaimDto>.Conflict(ex.Message);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain error adding claim to user {UserId}", request.UserId);
            return Result<UserClaimDto>.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding user claim to user {UserId}", request.UserId);
            return Result<UserClaimDto>.InternalError("An error occurred while adding the claim");
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

