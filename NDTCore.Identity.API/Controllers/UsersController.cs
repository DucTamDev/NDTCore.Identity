using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NDTCore.Identity.API.Controllers.Base;
using NDTCore.Identity.Contracts.Common;
using NDTCore.Identity.Contracts.Features.Users.DTOs;
using NDTCore.Identity.Contracts.Features.Users.Requests;
using NDTCore.Identity.Contracts.Interfaces.Services;
using System.Security.Claims;

namespace NDTCore.Identity.API.Controllers;

/// <summary>
/// User management endpoints
/// </summary>
[ApiController]
[Route("api/users")]
public class UsersController : BaseApiController
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        IUserService userService,
        ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
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
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
            ?? User.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
            return Unauthorized(ApiResponse<UserDto>.FailureResponse("Invalid user", 401));

        var response = await _userService.GetUserByIdAsync(userId, cancellationToken);
        
        if (!response.Success)
            return StatusCode(response.StatusCode, response);

        return Ok(response);
    }

    /// <summary>
    /// Get paginated list of users (Admin only)
    /// </summary>
    /// <param name="request">Pagination and filter request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of users</returns>
    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<UserDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers(
        [FromQuery] GetUsersRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _userService.GetUsersAsync(request, cancellationToken);
        
        if (!response.Success)
            return StatusCode(response.StatusCode, response);

        return Ok(response);
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
        var response = await _userService.GetUserByIdAsync(id, cancellationToken);
        
        if (!response.Success)
            return StatusCode(response.StatusCode, response);

        return Ok(response);
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
        var response = await _userService.CreateUserAsync(request, cancellationToken);
        
        if (!response.Success)
            return StatusCode(response.StatusCode, response);

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
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
            ?? User.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
            return Unauthorized(ApiResponse<UserDto>.FailureResponse("Invalid user", 401));

        var response = await _userService.UpdateUserAsync(userId, request, cancellationToken);
        
        if (!response.Success)
            return StatusCode(response.StatusCode, response);

        return Ok(response);
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
        var response = await _userService.UpdateUserAsync(id, request, cancellationToken);
        
        if (!response.Success)
            return StatusCode(response.StatusCode, response);

        return Ok(response);
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
        var response = await _userService.DeleteUserAsync(id, cancellationToken);
        
        if (!response.Success)
            return StatusCode(response.StatusCode, response);

        return Ok(response);
    }
}
