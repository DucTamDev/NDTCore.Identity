using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NDTCore.Identity.Contracts.Settings;
using NDTCore.Identity.Contracts.Common.Results;
using NDTCore.Identity.Contracts.Features.Authentication.DTOs;
using NDTCore.Identity.Contracts.Features.Authentication.Responses;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Contracts.Interfaces.Infrastructure;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Application.Features.Authentication.Commands.RefreshToken;

/// <summary>
/// Handler for RefreshTokenCommand
/// </summary>
public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthenticationResponse>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;

    public RefreshTokenCommandHandler(
        UserManager<AppUser> userManager,
        IJwtTokenService jwtTokenService,
        IRefreshTokenRepository refreshTokenRepository,
        IOptions<JwtSettings> jwtOptions,
        ILogger<RefreshTokenCommandHandler> logger)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtSettings = jwtOptions.Value;
        _logger = logger;
    }

    public async Task<Result<AuthenticationResponse>> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing token refresh request");

        // Validate access token and get principal
        var principal = _jwtTokenService.GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal == null)
        {
            _logger.LogWarning("Token refresh failed - invalid access token");
            return Result<AuthenticationResponse>.Unauthorized("Invalid token");
        }

        // Get user ID from token
        var userIdClaim = principal.FindFirst(Domain.Constants.ClaimTypes.Subject)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
        {
            _logger.LogWarning("Token refresh failed - invalid user ID in token");
            return Result<AuthenticationResponse>.Unauthorized("Invalid token");
        }

        // Get user
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null || !user.IsActive)
        {
            _logger.LogWarning("Token refresh failed - user not found or inactive: {UserId}", userId);
            return Result<AuthenticationResponse>.Unauthorized("Invalid token");
        }

        // Validate refresh token
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);
        if (storedToken == null || !storedToken.IsActive || storedToken.IsRevoked ||
            storedToken.UserId != userId || storedToken.IsExpired)
        {
            _logger.LogWarning("Token refresh failed - invalid or expired refresh token for user: {UserId}", userId);
            return Result<AuthenticationResponse>.Unauthorized("Invalid refresh token");
        }

        // Validate JWT ID mapping
        var jwtIdFromToken = _jwtTokenService.GetJwtIdFromToken(request.AccessToken);
        if (storedToken.JwtId != jwtIdFromToken)
        {
            _logger.LogWarning("Token refresh failed - JWT ID mismatch detected for user: {UserId}", userId);
            // Security: Revoke all user tokens
            await _refreshTokenRepository.RevokeAllUserTokensAsync(userId, cancellationToken);
            return Result<AuthenticationResponse>.Unauthorized("Token mismatch detected");
        }

        // Revoke old token
        storedToken.IsRevoked = true;
        storedToken.RevokedAt = DateTime.UtcNow;
        storedToken.RevokedByIp = request.IpAddress;

        // Generate new tokens
        var roles = await _userManager.GetRolesAsync(user);
        var newAccessToken = _jwtTokenService.GenerateAccessToken(user, roles);
        var newRefreshToken = _jwtTokenService.GenerateRefreshToken();
        var newJwtId = _jwtTokenService.GetJwtIdFromToken(newAccessToken);

        storedToken.ReplacedByToken = newRefreshToken;
        await _refreshTokenRepository.UpdateAsync(storedToken, cancellationToken);

        // Save new refresh token
        var newRefreshTokenEntity = new AppUserRefreshToken
        {
            UserId = user.Id,
            Token = newRefreshToken,
            JwtId = newJwtId,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
            CreatedByIp = request.IpAddress
        };

        await _refreshTokenRepository.AddAsync(newRefreshTokenEntity, cancellationToken);

        _logger.LogInformation("Token refreshed successfully for user: {UserId}", user.Id);

        // Create response
        var response = new AuthenticationResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = _jwtTokenService.GetTokenExpirationTime(),
            User = new UserInfoDto
            {
                Id = user.Id,
                Email = user.Email!,
                UserName = user.UserName!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                Roles = roles.ToList()
            }
        };

        return Result<AuthenticationResponse>.Success(response, "Token refreshed successfully");
    }
}

