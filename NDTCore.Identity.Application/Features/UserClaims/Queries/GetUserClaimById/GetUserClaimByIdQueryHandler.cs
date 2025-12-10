using MediatR;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Common.Results;
using NDTCore.Identity.Contracts.Features.Claims.DTOs;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Application.Features.UserClaims.Queries.GetUserClaimById;

/// <summary>
/// Handler for retrieving a specific user claim by ID
/// </summary>
public class GetUserClaimByIdQueryHandler : IRequestHandler<GetUserClaimByIdQuery, Result<UserClaimDto>>
{
    private readonly IUserClaimRepository _userClaimRepository;
    private readonly ILogger<GetUserClaimByIdQueryHandler> _logger;

    public GetUserClaimByIdQueryHandler(
        IUserClaimRepository userClaimRepository,
        ILogger<GetUserClaimByIdQueryHandler> logger)
    {
        _userClaimRepository = userClaimRepository;
        _logger = logger;
    }

    public async Task<Result<UserClaimDto>> Handle(GetUserClaimByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var claim = await _userClaimRepository.GetByIdAsync(request.ClaimId, cancellationToken);
            if (claim == null)
                return Result<UserClaimDto>.NotFound($"User claim with ID '{request.ClaimId}' was not found");

            var dto = MapToUserClaimDto(claim);
            _logger.LogInformation("Retrieved user claim {ClaimId}", request.ClaimId);
            return Result<UserClaimDto>.Success(dto, "User claim retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user claim {ClaimId}", request.ClaimId);
            return Result<UserClaimDto>.InternalError("An error occurred while retrieving the user claim");
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

