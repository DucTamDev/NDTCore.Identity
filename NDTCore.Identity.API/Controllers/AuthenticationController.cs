using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NDTCore.Identity.API.Controllers.Base;
using NDTCore.Identity.Application.Features.Authentication.Commands.ChangePassword;
using NDTCore.Identity.Application.Features.Authentication.Commands.ForgotPassword;
using NDTCore.Identity.Application.Features.Authentication.Commands.Login;
using NDTCore.Identity.Application.Features.Authentication.Commands.RefreshToken;
using NDTCore.Identity.Application.Features.Authentication.Commands.Register;
using NDTCore.Identity.Application.Features.Authentication.Commands.ResetPassword;
using NDTCore.Identity.Contracts.Common.Responses;
using NDTCore.Identity.Contracts.Features.Authentication.Responses;

namespace NDTCore.Identity.API.Controllers;

/// <summary>
/// Authentication and authorization endpoints
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthenticationController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public AuthenticationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="command">Registration command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authentication response with tokens</returns>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthenticationResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(command, cancellationToken);
        var response = ApiResponse<AuthenticationResponse>.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Login user
    /// </summary>
    /// <param name="command">Login command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Authentication response with tokens</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthenticationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginCommand command,
        CancellationToken cancellationToken = default)
    {
        // Set server-side data
        var commandWithIp = command with { IpAddress = GetClientIpAddress() };

        var result = await _mediator.Send(commandWithIp, cancellationToken);
        var response = ApiResponse<AuthenticationResponse>.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Refresh access token
    /// </summary>
    /// <param name="command">Refresh token command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>New authentication response with tokens</returns>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthenticationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken(
        [FromBody] RefreshTokenCommand command,
        CancellationToken cancellationToken = default)
    {
        // Set server-side data
        var commandWithIp = command with { IpAddress = GetClientIpAddress() };

        var result = await _mediator.Send(commandWithIp, cancellationToken);
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
        // TODO: Create LogoutCommand when needed
        // For now, keep using the repository directly
        var userId = GetCurrentUserId();

        // This would be better as a command, but keeping simple for now
        var errorResult = ApiResponse.Success("Logout endpoint - to be implemented with LogoutCommand");
        return StatusCode(errorResult.StatusCode, errorResult);
    }

    /// <summary>
    /// Change password
    /// </summary>
    /// <param name="command">Change password command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordCommand command,
        CancellationToken cancellationToken = default)
    {
        // Set server-side data (UserId from authenticated user)
        var commandWithUserId = command with { UserId = GetCurrentUserId() };

        var result = await _mediator.Send(commandWithUserId, cancellationToken);
        var response = ApiResponse.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Request password reset
    /// </summary>
    /// <param name="command">Forgot password command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(command, cancellationToken);
        var response = ApiResponse.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Reset password with token
    /// </summary>
    /// <param name="command">Reset password command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(command, cancellationToken);
        var response = ApiResponse.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }
}

