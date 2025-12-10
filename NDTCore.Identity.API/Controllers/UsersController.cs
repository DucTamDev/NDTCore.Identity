using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NDTCore.Identity.API.Controllers.Base;
using NDTCore.Identity.Application.Features.Authentication.Queries.GetCurrentUser;
using NDTCore.Identity.Application.Features.Users.Commands.CreateUser;
using NDTCore.Identity.Application.Features.Users.Commands.DeleteUser;
using NDTCore.Identity.Application.Features.Users.Commands.UpdateUser;
using NDTCore.Identity.Application.Features.Users.Queries.GetUserById;
using NDTCore.Identity.Application.Features.Users.Queries.GetUsersList;
using NDTCore.Identity.Contracts.Common.Responses;
using NDTCore.Identity.Contracts.Features.Users.DTOs;

namespace NDTCore.Identity.API.Controllers;

/// <summary>
/// User management endpoints
/// </summary>
[ApiController]
[Route("api/users")]
public class UsersController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
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
        var query = new GetCurrentUserQuery
        {
            UserId = GetCurrentUserId()
        };

        var result = await _mediator.Send(query, cancellationToken);
        var response = ApiResponse<UserDto>.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Get paginated list of users (Admin only)
    /// </summary>
    /// <param name="query">Pagination and filter query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of users</returns>
    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(PagedApiResponse<UserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers(
        [FromQuery] GetUsersListQuery query,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(query, cancellationToken);
        var response = PagedApiResponse<UserDto>.FromResult(result);
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
        var query = new GetUserByIdQuery { UserId = id };

        var result = await _mediator.Send(query, cancellationToken);
        var response = ApiResponse<UserDto>.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Create a new user (Admin only)
    /// </summary>
    /// <param name="command">Create user command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created user</returns>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUser(
        [FromBody] CreateUserCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(command, cancellationToken);
        var response = ApiResponse<Guid>.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Update current user profile
    /// </summary>
    /// <param name="command">Update user command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated user</returns>
    [HttpPut("me")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateCurrentUser(
        [FromBody] UpdateUserCommand command,
        CancellationToken cancellationToken = default)
    {
        // Set server-side data (UserId from authenticated user)
        var commandWithUserId = command with { UserId = GetCurrentUserId() };

        var result = await _mediator.Send(commandWithUserId, cancellationToken);
        var response = ApiResponse.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Update user by ID (Admin only)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="command">Update user command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated user</returns>
    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUser(
        [FromRoute] Guid id,
        [FromBody] UpdateUserCommand command,
        CancellationToken cancellationToken = default)
    {
        // Set UserId from route parameter
        var commandWithId = command with { UserId = id };

        var result = await _mediator.Send(commandWithId, cancellationToken);
        var response = ApiResponse.FromResult(result);
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
        var command = new DeleteUserCommand { UserId = id };

        var result = await _mediator.Send(command, cancellationToken);
        var response = ApiResponse.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }
}
