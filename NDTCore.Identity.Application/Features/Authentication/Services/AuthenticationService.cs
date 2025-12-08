using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NDTCore.Identity.Contracts.Common.Results;
using NDTCore.Identity.Contracts.Configuration;
using NDTCore.Identity.Contracts.Features.Authentication.DTOs;
using NDTCore.Identity.Contracts.Features.Authentication.Requests;
using NDTCore.Identity.Contracts.Features.Authentication.Responses;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Contracts.Interfaces.Services;
using NDTCore.Identity.Domain.Constants;
using NDTCore.Identity.Domain.Entities;
using NDTCore.Identity.Domain.Exceptions;

namespace NDTCore.Identity.Application.Features.Authentication.Services;

/// <summary>
/// Service for handling authentication operations
/// </summary>
public class AuthenticationService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly JwtSettings _jwtSettings;
    private readonly TokenValidationSettings _tokenValidationSettings;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        IJwtTokenService jwtTokenService,
        IRefreshTokenRepository refreshTokenRepository,
        IOptions<JwtSettings> jwtOptions,
        IOptions<TokenValidationSettings> tokenValidationOptions,
        ILogger<AuthenticationService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtSettings = jwtOptions.Value;
        _tokenValidationSettings = tokenValidationOptions.Value;
        _logger = logger;
    }

    public async Task<Result<AuthenticationResponse>> LoginAsync(LoginRequest request, string ipAddress, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return Result<AuthenticationResponse>.Unauthorized("Invalid email or password");

            if (!user.IsActive)
                return Result<AuthenticationResponse>.Forbidden("Account is inactive");

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                    return Result<AuthenticationResponse>.Forbidden("Account is locked out");

                return Result<AuthenticationResponse>.Unauthorized("Invalid email or password");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _jwtTokenService.GenerateAccessToken(user, roles);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();
            var jwtId = _jwtTokenService.GetJwtIdFromToken(accessToken);

            // Check active token count and enforce limit
            var activeTokenCount = await _refreshTokenRepository.GetActiveTokenCountAsync(user.Id, cancellationToken);
            var maxTokens = _tokenValidationSettings.MaxActiveTokensPerUser;
            if (activeTokenCount >= maxTokens)
            {
                // Revoke oldest tokens to maintain limit
                var activeTokens = await _refreshTokenRepository.GetActiveTokensByUserIdAsync(user.Id, cancellationToken);
                var tokensToKeep = maxTokens - 1; // Keep max - 1, revoke oldest
                var tokensToRevoke = activeTokens
                    .OrderBy(t => t.CreatedAt)
                    .Take(activeTokens.Count - tokensToKeep)
                    .ToList();

                foreach (var token in tokensToRevoke)
                {
                    await _refreshTokenRepository.RevokeTokenAsync(token.Token, SystemConstants.SystemOperations.TokenLimitRevocation, cancellationToken);
                }
            }

            // Save refresh token
            var refreshTokenEntity = new AppUserRefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                JwtId = jwtId,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
                CreatedByIp = ipAddress
            };

            await _refreshTokenRepository.AddAsync(refreshTokenEntity, cancellationToken);

            // Update last login
            user.LastLoginAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

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
                    Roles = roles.ToList(),
                }
            };

            return Result<AuthenticationResponse>.Success(response, "Login successful");
        }
        catch (ConflictException ex)
        {
            return Result<AuthenticationResponse>.Conflict(ex.Message);
        }
        catch (DomainException ex)
        {
            return Result<AuthenticationResponse>.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return Result<AuthenticationResponse>.InternalError("An error occurred during login");
        }
    }

    public async Task<Result<AuthenticationResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
                return Result<AuthenticationResponse>.Conflict("Email already exists");

            var existingUserName = await _userManager.FindByNameAsync(request.UserName);
            if (existingUserName != null)
                return Result<AuthenticationResponse>.Conflict("Username already exists");

            var user = new AppUser
            {
                UserName = request.UserName,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                IsActive = true,
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var validationErrors = result.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.Description).ToList());

                return Result<AuthenticationResponse>.BadRequest(
                    message: "One or more validation errors occurred",
                    errorCode: "VALIDATION_ERROR",
                    validationErrors: validationErrors);
            }

            // Assign default role
            await _userManager.AddToRoleAsync(user, SystemConstants.Roles.User);

            // Auto-login after registration
            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _jwtTokenService.GenerateAccessToken(user, roles);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();
            var jwtId = _jwtTokenService.GetJwtIdFromToken(accessToken);

            // Check active token count and enforce limit (for new users, this will be 0)
            var activeTokenCount = await _refreshTokenRepository.GetActiveTokenCountAsync(user.Id, cancellationToken);
            var maxTokens = _tokenValidationSettings.MaxActiveTokensPerUser;
            if (activeTokenCount >= maxTokens)
            {
                var activeTokens = await _refreshTokenRepository.GetActiveTokensByUserIdAsync(user.Id, cancellationToken);
                var tokensToKeep = maxTokens - 1; // Keep max - 1, revoke oldest
                var tokensToRevoke = activeTokens
                    .OrderBy(t => t.CreatedAt)
                    .Take(activeTokens.Count - tokensToKeep)
                    .ToList();

                foreach (var token in tokensToRevoke)
                {
                    await _refreshTokenRepository.RevokeTokenAsync(token.Token, SystemConstants.SystemOperations.TokenLimitRevocation, cancellationToken);
                }
            }

            var refreshTokenEntity = new AppUserRefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                JwtId = jwtId,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
                CreatedByIp = SystemConstants.SystemOperations.Registration
            };

            await _refreshTokenRepository.AddAsync(refreshTokenEntity, cancellationToken);

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
                    Roles = roles.ToList()
                }
            };

            return Result<AuthenticationResponse>.Success(response, "Registration successful");
        }
        catch (ConflictException ex)
        {
            return Result<AuthenticationResponse>.Conflict(ex.Message);
        }
        catch (DomainException ex)
        {
            return Result<AuthenticationResponse>.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration");
            return Result<AuthenticationResponse>.InternalError("An error occurred during registration");
        }
    }

    public async Task<Result<AuthenticationResponse>> RefreshTokenAsync(RefreshTokenRequest request, string ipAddress, CancellationToken cancellationToken = default)
    {
        try
        {
            var principal = _jwtTokenService.GetPrincipalFromExpiredToken(request.AccessToken);
            if (principal == null)
                return Result<AuthenticationResponse>.Unauthorized("Invalid token");

            var userIdClaim = principal.FindFirst(NDTCore.Identity.Domain.Constants.ClaimTypes.Subject)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                return Result<AuthenticationResponse>.Unauthorized("Invalid token");

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null || !user.IsActive)
                return Result<AuthenticationResponse>.Unauthorized("Invalid token");

            var storedToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);
            if (storedToken == null || !storedToken.IsActive || storedToken.IsRevoked || storedToken.UserId != userId || storedToken.IsExpired)
            {
                return Result<AuthenticationResponse>.Unauthorized("Invalid refresh token");
            }

            // Validate JWT ID mapping - security best practice
            var jwtIdFromToken = _jwtTokenService.GetJwtIdFromToken(request.AccessToken);
            if (storedToken.JwtId != jwtIdFromToken)
            {
                // Token mismatch detected - potential security issue
                // Revoke all user tokens as precaution
                await _refreshTokenRepository.RevokeAllUserTokensAsync(userId, cancellationToken);
                return Result<AuthenticationResponse>.Unauthorized("Token mismatch detected");
            }

            // Revoke old token
            storedToken.IsRevoked = true;
            storedToken.RevokedAt = DateTime.UtcNow;
            storedToken.RevokedByIp = ipAddress;

            await _refreshTokenRepository.UpdateAsync(storedToken, cancellationToken);

            // Generate new tokens
            var roles = await _userManager.GetRolesAsync(user);
            var newAccessToken = _jwtTokenService.GenerateAccessToken(user, roles);
            var newRefreshToken = _jwtTokenService.GenerateRefreshToken();
            var jwtId = _jwtTokenService.GetJwtIdFromToken(newAccessToken);

            storedToken.ReplacedByToken = newRefreshToken;
            await _refreshTokenRepository.UpdateAsync(storedToken, cancellationToken);

            var newRefreshTokenEntity = new AppUserRefreshToken
            {
                UserId = user.Id,
                Token = newRefreshToken,
                JwtId = jwtId,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
                CreatedByIp = ipAddress
            };

            await _refreshTokenRepository.AddAsync(newRefreshTokenEntity, cancellationToken);

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

            return Result<AuthenticationResponse>.Success(response, "Token refreshed");
        }
        catch (ConflictException ex)
        {
            return Result<AuthenticationResponse>.Conflict(ex.Message);
        }
        catch (DomainException ex)
        {
            return Result<AuthenticationResponse>.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            return Result<AuthenticationResponse>.InternalError("An error occurred while refreshing the token");
        }
    }

    public async Task<Result> ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return Result.NotFound($"User with ID '{userId}' was not found");

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

            if (!result.Succeeded)
            {
                var validationErrors = result.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.Description).ToList());

                return Result.BadRequest(
                    message: "One or more validation errors occurred",
                    errorCode: ErrorCodes.ValidationError,
                    validationErrors: validationErrors);
            }

            return Result.Success("Password changed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password for user: {UserId}", userId);
            return Result.InternalError("An error occurred while changing the password");
        }
    }

    public async Task<Result> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                // Security: Don't reveal if email exists
                return Result.Success("If the email exists, a reset link has been sent");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // TODO: Send email with reset token
            // await _emailService.SendPasswordResetEmailAsync(user.Email, token);

            return Result.Success("If the email exists, a reset link has been sent");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing forgot password request");
            return Result.InternalError("An error occurred while processing the request");
        }
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return Result.BadRequest("Invalid request");

            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);

            if (!result.Succeeded)
            {
                var validationErrors = result.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.Description).ToList());

                return Result.BadRequest(
                    message: "One or more validation errors occurred",
                    errorCode: ErrorCodes.ValidationError,
                    validationErrors: validationErrors);
            }

            return Result.Success("Password reset successful");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting password");
            return Result.InternalError("An error occurred while resetting the password");
        }
    }

    public async Task<Result> LogoutAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            await _refreshTokenRepository.RevokeAllUserTokensAsync(userId, cancellationToken);
            return Result.Success("Logged out successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout for user: {UserId}", userId);
            return Result.InternalError("An error occurred during logout");
        }
    }
}