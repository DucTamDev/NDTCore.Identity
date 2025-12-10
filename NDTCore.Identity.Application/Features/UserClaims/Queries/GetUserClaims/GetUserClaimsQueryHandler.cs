using MediatR;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Common.Results;
using NDTCore.Identity.Contracts.Features.Claims.DTOs;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Application.Features.UserClaims.Queries.GetUserClaims;

/// <summary>
/// Handler for retrieving all claims for a specific user
/// </summary>
public class GetUserClaimsQueryHandler : IRequestHandler<GetUserClaimsQuery, Result<List<UserClaimDto>>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserClaimRepository _userClaimRepository;
    private readonly ILogger<GetUserClaimsQueryHandler> _logger;

    public GetUserClaimsQueryHandler(
        IUserRepository userRepository,
        IUserClaimRepository userClaimRepository,
        ILogger<GetUserClaimsQueryHandler> logger)
    {
        _userRepository = userRepository;
        _userClaimRepository = userClaimRepository;
        _logger = logger;
    }

    public async Task<Result<List<UserClaimDto>>> Handle(GetUserClaimsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
                return Result<List<UserClaimDto>>.NotFound($"User with ID '{request.UserId}' was not found");

            var claims = await _userClaimRepository.GetClaimsByUserIdAsync(request.UserId, cancellationToken);
            var dtos = claims.Select(MapToUserClaimDto).ToList();

            _logger.LogInformation("Retrieved {Count} claims for user {UserId}", dtos.Count, request.UserId);
            return Result<List<UserClaimDto>>.Success(dtos, "User claims retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user claims for user {UserId}", request.UserId);
            return Result<List<UserClaimDto>>.InternalError("An error occurred while retrieving user claims");
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

