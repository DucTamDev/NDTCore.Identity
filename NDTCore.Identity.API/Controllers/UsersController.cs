using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NDTCore.Identity.API.Controllers.Base;
using NDTCore.Identity.Contracts.Common;
using NDTCore.Identity.Contracts.Extensions;
using NDTCore.Identity.Contracts.Features.Users.DTOs;
using NDTCore.Identity.Contracts.Features.Users.Requests;
using NDTCore.Identity.Contracts.Interfaces.Services;

namespace NDTCore.Identity.API.Controllers;

/// <summary>
/// User management endpoints
/// </summary>
[ApiController]
[Route("api/users")]
public class UsersController : BaseApiController
{
    private readonly IUserService _userService;

    public UsersController(
        IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Get current user profile
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Current user details</returns>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken = default)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
            ?? User.FindFirst(NDTCore.Identity.Domain.Constants.ClaimTypes.Subject)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
        {
            var errorResult = ApiResponse<UserDto>.Unauthorized("Invalid user");
            return StatusCode(errorResult.StatusCode, errorResult);
        }

        var result = await _userService.GetUserByIdAsync(userId, cancellationToken);
        var response = ApiResponse<UserDto>.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Get paginated list of users (Admin only)
    /// </summary>
    /// <param name="request">Pagination and filter request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of users</returns>
    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(PagedApiResponse<UserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers(
        [FromQuery] GetUsersRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await _userService.GetUsersAsync(request, cancellationToken);
        var response = result.ToPagedApiResponse();
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User details</returns>
    [HttpGet("{id}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await _userService.GetUserByIdAsync(id, cancellationToken);
        var response = ApiResponse<UserDto>.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Create a new user (Admin only)
    /// </summary>
    /// <param name="request">Create user request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created user</returns>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUser(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await _userService.CreateUserAsync(request, cancellationToken);
        var response = ApiResponse<UserDto>.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Update current user profile
    /// </summary>
    /// <param name="request">Update user request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated user</returns>
    [HttpPut("me")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateCurrentUser(
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
            ?? User.FindFirst(NDTCore.Identity.Domain.Constants.ClaimTypes.Subject)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
        {
            var errorResult = ApiResponse<UserDto>.Unauthorized("Invalid user");
            return StatusCode(errorResult.StatusCode, errorResult);
        }

        var result = await _userService.UpdateUserAsync(userId, request, cancellationToken);
        var response = ApiResponse<UserDto>.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Update user by ID (Admin only)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="request">Update user request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated user</returns>
    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUser(
        [FromRoute] Guid id,
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await _userService.UpdateUserAsync(id, request, cancellationToken);
        var response = ApiResponse<UserDto>.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Delete user (soft delete, Admin only)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await _userService.DeleteUserAsync(id, cancellationToken);
        var response = ApiResponse.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }
}
