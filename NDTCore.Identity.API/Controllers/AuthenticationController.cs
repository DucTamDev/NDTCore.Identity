using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NDTCore.Identity.API.Controllers.Base;
using NDTCore.Identity.Contracts.Common;
using NDTCore.Identity.Contracts.Features.Authentication.Requests;
using NDTCore.Identity.Contracts.Features.Authentication.Responses;
using NDTCore.Identity.Contracts.Interfaces.Services;

namespace NDTCore.Identity.API.Controllers;

/// <summary>
/// Authentication and authorization endpoints
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthenticationController : BaseApiController
{
    private readonly IAuthService _authService;

    public AuthenticationController(
        IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="request">Registration request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authentication response with tokens</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthenticationResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await _authService.RegisterAsync(request, cancellationToken);
        var response = ApiResponse<AuthenticationResponse>.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Login user
    /// </summary>
    /// <param name="request">Login request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authentication response with tokens</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthenticationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        var result = await _authService.LoginAsync(request, ipAddress, cancellationToken);
        var response = ApiResponse<AuthenticationResponse>.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Refresh access token
    /// </summary>
    /// <param name="request">Refresh token request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>New authentication response with tokens</returns>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthenticationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken = default)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        var result = await _authService.RefreshTokenAsync(request, ipAddress, cancellationToken);
        var response = ApiResponse<AuthenticationResponse>.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Logout user
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken = default)
    {
        var userIdClaim = User.FindFirst(NDTCore.Identity.Domain.Constants.ClaimTypes.Subject)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
        {
            var errorResult = ApiResponse.Unauthorized("Invalid user");
            return StatusCode(errorResult.StatusCode, errorResult);
        }

        var result = await _authService.LogoutAsync(userId, cancellationToken);
        var response = ApiResponse.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Change password
    /// </summary>
    /// <param name="request">Change password request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        var userIdClaim = User.FindFirst(NDTCore.Identity.Domain.Constants.ClaimTypes.Subject)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
        {
            var errorResult = ApiResponse.Unauthorized("Invalid user");
            return StatusCode(errorResult.StatusCode, errorResult);
        }

        var result = await _authService.ChangePasswordAsync(userId, request, cancellationToken);
        var response = ApiResponse.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Request password reset
    /// </summary>
    /// <param name="request">Forgot password request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await _authService.ForgotPasswordAsync(request, cancellationToken);
        var response = ApiResponse.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Reset password with token
    /// </summary>
    /// <param name="request">Reset password request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await _authService.ResetPasswordAsync(request, cancellationToken);
        var response = ApiResponse.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }
}

