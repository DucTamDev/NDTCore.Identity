using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NDTCore.Identity.API.Controllers.Base;
using NDTCore.Identity.Contracts.Common;
using NDTCore.Identity.Contracts.Features.Permissions.DTOs;
using NDTCore.Identity.Contracts.Features.Permissions.Requests;
using NDTCore.Identity.Contracts.Interfaces.Services;

namespace NDTCore.Identity.API.Controllers;

/// <summary>
/// Permission management endpoints
/// </summary>
[ApiController]
[Route("api/permissions")]
[Authorize(Policy = "AdminOnly")]
public class PermissionsController : BaseApiController
{
    private readonly IPermissionService _permissionService;
    private readonly ILogger<PermissionsController> _logger;

    public PermissionsController(
        IPermissionService permissionService,
        ILogger<PermissionsController> logger)
    {
        _permissionService = permissionService;
        _logger = logger;
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
        var response = await _permissionService.GetAllPermissionsAsync(cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Get permissions grouped by category
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Grouped permissions</returns>
    [HttpGet("grouped")]
    [ProducesResponseType(typeof(ApiResponse<Dictionary<string, List<PermissionDto>>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetGroupedPermissions(CancellationToken cancellationToken = default)
    {
        var response = await _permissionService.GetGroupedPermissionsAsync(cancellationToken);
        return Ok(response);
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
        var response = await _permissionService.GetUserPermissionsAsync(userId, cancellationToken);

        if (!response.Success)
            return StatusCode(response.StatusCode, response);

        return Ok(response);
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
        var response = await _permissionService.GetRolePermissionsAsync(roleId, cancellationToken);

        if (!response.Success)
            return StatusCode(response.StatusCode, response);

        return Ok(response);
    }

    /// <summary>
    /// Assign permissions to a role (via RoleClaims)
    /// </summary>
    /// <param name="request">Assign permission request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpPost("roles/assign")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignPermissionsToRole(
        [FromBody] AssignPermissionRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _permissionService.AssignPermissionsToRoleAsync(request, cancellationToken);

        if (!response.Success)
            return StatusCode(response.StatusCode, response);

        return Ok(response);
    }

    /// <summary>
    /// Revoke permissions from a role
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <param name="permissions">List of permission names to revoke</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpPost("roles/{roleId}/revoke")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RevokePermissionsFromRole(
        [FromRoute] Guid roleId,
        [FromBody] List<string> permissions,
        CancellationToken cancellationToken = default)
    {
        if (permissions == null || !permissions.Any())
            return BadRequest(ApiResponse.FailureResponse("At least one permission must be specified", 400));

        var response = await _permissionService.RevokePermissionsFromRoleAsync(roleId, permissions, cancellationToken);

        if (!response.Success)
            return StatusCode(response.StatusCode, response);

        return Ok(response);
    }
}

