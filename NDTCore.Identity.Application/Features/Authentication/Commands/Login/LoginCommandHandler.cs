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

namespace NDTCore.Identity.Application.Features.Authentication.Commands.Login;

/// <summary>
/// Handler for LoginCommand
/// </summary>
public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthenticationResponse>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly JwtSettings _jwtSettings;
    private readonly TokenValidationSettings _tokenValidationSettings;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        IJwtTokenService jwtTokenService,
        IRefreshTokenRepository refreshTokenRepository,
        IOptions<JwtSettings> jwtOptions,
        IOptions<TokenValidationSettings> tokenValidationOptions,
        ILogger<LoginCommandHandler> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtSettings = jwtOptions.Value;
        _tokenValidationSettings = tokenValidationOptions.Value;
        _logger = logger;
    }

    public async Task<Result<AuthenticationResponse>> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing login request for email: {Email}", request.Email);

        // Find user by email
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            _logger.LogWarning("Login failed - user not found: {Email}", request.Email);
            return Result<AuthenticationResponse>.Unauthorized("Invalid email or password");
        }

        // Check if user is active
        if (!user.IsActive)
        {
            _logger.LogWarning("Login failed - user inactive: {Email}", request.Email);
            return Result<AuthenticationResponse>.Forbidden("Account is inactive");
        }

        // Verify password
        var result = await _signInManager.CheckPasswordSignInAsync(
            user,
            request.Password,
            lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
            {
                _logger.LogWarning("Login failed - account locked: {Email}", request.Email);
                return Result<AuthenticationResponse>.Forbidden("Account is locked out");
            }

            _logger.LogWarning("Login failed - invalid password: {Email}", request.Email);
            return Result<AuthenticationResponse>.Unauthorized("Invalid email or password");
        }

        // Get user roles
        var roles = await _userManager.GetRolesAsync(user);

        // Generate tokens
        var accessToken = _jwtTokenService.GenerateAccessToken(user, roles);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();
        var jwtId = _jwtTokenService.GetJwtIdFromToken(accessToken);

        // Check and enforce token limit
        var activeTokenCount = await _refreshTokenRepository.GetActiveTokenCountAsync(user.Id, cancellationToken);
        var maxTokens = _tokenValidationSettings.MaxActiveTokensPerUser;

        if (activeTokenCount >= maxTokens)
        {
            // Revoke oldest tokens to maintain limit
            var activeTokens = await _refreshTokenRepository.GetActiveTokensByUserIdAsync(user.Id, cancellationToken);
            var tokensToKeep = maxTokens - 1;
            var tokensToRevoke = activeTokens
                .OrderBy(t => t.CreatedAt)
                .Take(activeTokens.Count - tokensToKeep)
                .ToList();

            foreach (var token in tokensToRevoke)
            {
                await _refreshTokenRepository.RevokeTokenAsync(
                    token.Token,
                    Domain.Constants.SystemConstants.SystemOperations.TokenLimitRevocation,
                    cancellationToken);
            }
        }

        // Save new refresh token
        var refreshTokenEntity = new AppUserRefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            JwtId = jwtId,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
            CreatedByIp = request.IpAddress
        };

        await _refreshTokenRepository.AddAsync(refreshTokenEntity, cancellationToken);

        // Update last login time
        user.LastLoginAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        _logger.LogInformation("Login successful for user: {UserId}", user.Id);

        // Create response
        var response = new AuthenticationResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = _jwtTokenService.GetTokenExpirationTime(),
            User = new UserInfoDto
            {
                Id = user.Id,
                Email = user.Email!,
                UserName = user.UserName!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                AvatarUrl = user.AvatarUrl,
                PhoneNumber = user.PhoneNumber,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                Roles = roles.ToList()
            }
        };

        return Result<AuthenticationResponse>.Success(response, "Login successful");
    }
}

