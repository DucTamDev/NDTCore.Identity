using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NDTCore.Identity.API.Controllers.Base;
using NDTCore.Identity.Application.Features.Roles.Commands.CreateRole;
using NDTCore.Identity.Application.Features.Roles.Commands.DeleteRole;
using NDTCore.Identity.Application.Features.Roles.Commands.UpdateRole;
using NDTCore.Identity.Application.Features.Roles.Queries.GetRoleById;
using NDTCore.Identity.Application.Features.Roles.Queries.GetRolesList;
using NDTCore.Identity.Application.Features.UserRoles.Commands.RemoveRoleFromUser;
using NDTCore.Identity.Contracts.Common.Responses;
using NDTCore.Identity.Contracts.Features.Roles.DTOs;

namespace NDTCore.Identity.API.Controllers;

/// <summary>
/// Role management endpoints
/// </summary>
[ApiController]
[Route("api/roles")]
[Authorize(Policy = "AdminOnly")]
public class RolesController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public RolesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all roles
    /// </summary>
    /// <param name="query">Pagination and filter query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of roles</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedApiResponse<RoleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllRoles(
        [FromQuery] GetRolesListQuery query,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(query, cancellationToken);
        var response = PagedApiResponse<RoleDto>.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Get role by ID
    /// </summary>
    /// <param name="id">Role ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Role details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<RoleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<RoleDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRoleById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetRoleByIdQuery { RoleId = id };

        var result = await _mediator.Send(query, cancellationToken);
        var response = ApiResponse<RoleDto>.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Create a new role
    /// </summary>
    /// <param name="command">Create role command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created role</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateRole(
        [FromBody] CreateRoleCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(command, cancellationToken);
        var response = ApiResponse<Guid>.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Update an existing role
    /// </summary>
    /// <param name="id">Role ID</param>
    /// <param name="command">Update role command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated role</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateRole(
        [FromRoute] Guid id,
        [FromBody] UpdateRoleCommand command,
        CancellationToken cancellationToken = default)
    {
        // Set RoleId from route parameter
        var commandWithId = command with { RoleId = id };

        var result = await _mediator.Send(commandWithId, cancellationToken);
        var response = ApiResponse.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Delete a role
    /// </summary>
    /// <param name="id">Role ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRole(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteRoleCommand { RoleId = id };

        var result = await _mediator.Send(command, cancellationToken);
        var response = ApiResponse.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Remove role from user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="roleId">Role ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpDelete("{roleId}/users/{userId}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveRoleFromUser(
        [FromRoute] Guid userId,
        [FromRoute] Guid roleId,
        CancellationToken cancellationToken = default)
    {
        var command = new RemoveRoleFromUserCommand
        {
            UserId = userId,
            RoleId = roleId
        };

        var result = await _mediator.Send(command, cancellationToken);
        var response = ApiResponse.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }
}

