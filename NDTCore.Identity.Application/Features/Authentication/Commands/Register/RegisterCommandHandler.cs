using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NDTCore.Identity.Contracts.Settings;
using NDTCore.Identity.Contracts.Common.Results;
using NDTCore.Identity.Contracts.Features.Authentication.DTOs;
using NDTCore.Identity.Contracts.Features.Authentication.Responses;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Domain.Constants;
using NDTCore.Identity.Contracts.Interfaces.Infrastructure;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Application.Features.Authentication.Commands.Register;

/// <summary>
/// Handler for RegisterCommand
/// </summary>
public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthenticationResponse>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<RegisterCommandHandler> _logger;

    public RegisterCommandHandler(
        UserManager<AppUser> userManager,
        IJwtTokenService jwtTokenService,
        IRefreshTokenRepository refreshTokenRepository,
        IOptions<JwtSettings> jwtOptions,
        ILogger<RegisterCommandHandler> logger)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtSettings = jwtOptions.Value;
        _logger = logger;
    }

    public async Task<Result<AuthenticationResponse>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing registration for email: {Email}", request.Email);

        // Check if email already exists
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            _logger.LogWarning("Registration failed - email already exists: {Email}", request.Email);
            return Result<AuthenticationResponse>.Conflict("Email already exists");
        }

        // Check if username already exists
        var existingUserName = await _userManager.FindByNameAsync(request.UserName);
        if (existingUserName != null)
        {
            _logger.LogWarning("Registration failed - username already exists: {UserName}", request.UserName);
            return Result<AuthenticationResponse>.Conflict("Username already exists");
        }

        // Create new user
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

            _logger.LogWarning("Registration failed - validation errors for email: {Email}", request.Email);
            return Result<AuthenticationResponse>.BadRequest(
                message: "One or more validation errors occurred",
                errorCode: ErrorCodes.ValidationError,
                validationErrors: validationErrors);
        }

        // Assign default role
        await _userManager.AddToRoleAsync(user, SystemConstants.Roles.User);

        _logger.LogInformation("User registered successfully: {UserId}", user.Id);

        // Auto-login after registration
        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtTokenService.GenerateAccessToken(user, roles);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();
        var jwtId = _jwtTokenService.GetJwtIdFromToken(accessToken);

        // Save refresh token
        var refreshTokenEntity = new AppUserRefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            JwtId = jwtId,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
            CreatedByIp = SystemConstants.SystemOperations.Registration
        };

        await _refreshTokenRepository.AddAsync(refreshTokenEntity, cancellationToken);

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
                Roles = roles.ToList()
            }
        };

        return Result<AuthenticationResponse>.Success(response, "Registration successful");
    }
}

