using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NDTCore.Identity.API.Controllers.Base;
using NDTCore.Identity.Application.Features.UserRoles.Commands.AssignRoleToUser;
using NDTCore.Identity.Application.Features.UserRoles.Commands.RemoveRoleFromUser;
using NDTCore.Identity.Application.Features.UserRoles.Queries.GetUserRole;
using NDTCore.Identity.Application.Features.UserRoles.Queries.GetUserRoles;
using NDTCore.Identity.Application.Features.UserRoles.Queries.GetUserRolesByRoleId;
using NDTCore.Identity.Application.Features.UserRoles.Queries.GetUserRolesByUserId;
using NDTCore.Identity.Contracts.Common.Responses;
using NDTCore.Identity.Contracts.Features.UserRoles.DTOs;

namespace NDTCore.Identity.API.Controllers;

/// <summary>
/// User-role assignment management endpoints
/// </summary>
[ApiController]
[Route("api/user-roles")]
[Authorize(Policy = "AdminOnly")]
public class UserRolesController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public UserRolesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get paginated list of user-role assignments
    /// </summary>
    /// <param name="query">Filter and pagination query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of user-role assignments</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedApiResponse<UserRoleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserRoles(
        [FromQuery] GetUserRolesQuery query,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(query, cancellationToken);
        var response = PagedApiResponse<UserRoleDto>.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Get all roles assigned to a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of user-role assignments</returns>
    [HttpGet("users/{userId}")]
    [ProducesResponseType(typeof(ApiResponse<List<UserRoleDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserRolesByUserId(
        [FromRoute] Guid userId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetUserRolesByUserIdQuery { UserId = userId };
        var result = await _mediator.Send(query, cancellationToken);
        var response = ApiResponse<List<UserRoleDto>>.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Get all users assigned to a role
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of user-role assignments</returns>
    [HttpGet("roles/{roleId}")]
    [ProducesResponseType(typeof(ApiResponse<List<UserRoleDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserRolesByRoleId(
        [FromRoute] Guid roleId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetUserRolesByRoleIdQuery { RoleId = roleId };
        var result = await _mediator.Send(query, cancellationToken);
        var response = ApiResponse<List<UserRoleDto>>.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Get specific user-role assignment
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="roleId">Role ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User-role assignment details</returns>
    [HttpGet("{userId}/{roleId}")]
    [ProducesResponseType(typeof(ApiResponse<UserRoleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserRole(
        [FromRoute] Guid userId,
        [FromRoute] Guid roleId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetUserRoleQuery { UserId = userId, RoleId = roleId };
        var result = await _mediator.Send(query, cancellationToken);
        var response = ApiResponse<UserRoleDto>.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Assign a role to a user
    /// </summary>
    /// <param name="command">Assign role command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created user-role assignment</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<UserRoleDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignRoleToUser(
        [FromBody] AssignRoleToUserCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(command, cancellationToken);
        var response = ApiResponse<UserRoleDto>.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Remove a role from a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="roleId">Role ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpDelete("{userId}/{roleId}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveRoleFromUser(
        [FromRoute] Guid userId,
        [FromRoute] Guid roleId,
        CancellationToken cancellationToken = default)
    {
        var command = new RemoveRoleFromUserCommand { UserId = userId, RoleId = roleId };
        var result = await _mediator.Send(command, cancellationToken);
        var response = ApiResponse.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }
}

