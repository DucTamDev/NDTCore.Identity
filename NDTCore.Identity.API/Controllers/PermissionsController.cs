using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NDTCore.Identity.API.Controllers.Base;
using NDTCore.Identity.Application.Features.Permissions.Queries.GetAllPermissions;
using NDTCore.Identity.Application.Features.Permissions.Queries.GetUserPermissions;
using NDTCore.Identity.Application.Features.Permissions.Queries.GetRolePermissions;
using NDTCore.Identity.Contracts.Common.Responses;
using NDTCore.Identity.Contracts.Features.Permissions.DTOs;

namespace NDTCore.Identity.API.Controllers;

/// <summary>
/// Permission management endpoints
/// </summary>
[ApiController]
[Route("api/permissions")]
[Authorize(Policy = "AdminOnly")]
public class PermissionsController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public PermissionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all available permissions in the system
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all permissions</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<PermissionDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPermissions(CancellationToken cancellationToken = default)
    {
        var query = new GetAllPermissionsQuery { GroupByModule = false };

        var result = await _mediator.Send(query, cancellationToken);
        var response = ApiResponse<List<PermissionDto>>.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Get permissions grouped by module
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Grouped permissions</returns>
    [HttpGet("grouped")]
    [ProducesResponseType(typeof(ApiResponse<List<PermissionDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetGroupedPermissions(CancellationToken cancellationToken = default)
    {
        var query = new GetAllPermissionsQuery { GroupByModule = true };
        var result = await _mediator.Send(query, cancellationToken);
        var response = ApiResponse<List<PermissionDto>>.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Get effective permissions for a user (direct + role permissions)
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User permissions details</returns>
    [HttpGet("users/{userId}")]
    [ProducesResponseType(typeof(ApiResponse<UserPermissionsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserPermissions(
        [FromRoute] Guid userId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetUserPermissionsQuery { UserId = userId };

        var result = await _mediator.Send(query, cancellationToken);
        var response = ApiResponse<UserPermissionsDto>.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Get permissions assigned to a role
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of role permissions</returns>
    [HttpGet("roles/{roleId}")]
    [ProducesResponseType(typeof(ApiResponse<List<string>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRolePermissions(
        [FromRoute] Guid roleId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetRolePermissionsQuery { RoleId = roleId };

        var result = await _mediator.Send(query, cancellationToken);
        var response = ApiResponse<List<string>>.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }

    // Note: Permission assignment/revocation should be done through RoleClaims endpoints
    // Use ClaimsController endpoints: POST /api/roles/{roleId}/claims and DELETE /api/role-claims/{claimId}
}

