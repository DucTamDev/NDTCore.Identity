using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NDTCore.Identity.API.Controllers.Base;
using NDTCore.Identity.Contracts.Common;
using NDTCore.Identity.Contracts.Features.Authentication.Requests;
using NDTCore.Identity.Contracts.Features.Authentication.Responses;
using NDTCore.Identity.Contracts.Interfaces.Services;
using System.Security.Claims;

namespace NDTCore.Identity.API.Controllers;

/// <summary>
/// Authentication and authorization endpoints
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthenticationController : BaseApiController
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(
        IAuthService authService,
        ILogger<AuthenticationController> logger)
    {
        _authService = authService;
        _logger = logger;
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
        var response = await _authService.RegisterAsync(request, cancellationToken);
        
        if (!response.Success)
            return StatusCode(response.StatusCode, response);

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
        var response = await _authService.LoginAsync(request, ipAddress, cancellationToken);
        
        if (!response.Success)
            return StatusCode(response.StatusCode, response);

        return Ok(response);
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
        var response = await _authService.RefreshTokenAsync(request, ipAddress, cancellationToken);
        
        if (!response.Success)
            return StatusCode(response.StatusCode, response);

        return Ok(response);
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
        var userIdClaim = User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
            return Unauthorized(ApiResponse.FailureResponse("Invalid user", 401));

        var response = await _authService.LogoutAsync(userId, cancellationToken);
        return Ok(response);
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
        var userIdClaim = User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
            return Unauthorized(ApiResponse.FailureResponse("Invalid user", 401));

        var response = await _authService.ChangePasswordAsync(userId, request, cancellationToken);
        
        if (!response.Success)
            return StatusCode(response.StatusCode, response);

        return Ok(response);
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
        var response = await _authService.ForgotPasswordAsync(request, cancellationToken);
        return Ok(response);
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
        var response = await _authService.ResetPasswordAsync(request, cancellationToken);
        
        if (!response.Success)
            return StatusCode(response.StatusCode, response);

        return Ok(response);
    }
}

