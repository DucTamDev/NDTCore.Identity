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

    public PermissionsController(
        IPermissionService permissionService)
    {
        _permissionService = permissionService;
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
        var result = await _permissionService.GetAllPermissionsAsync(cancellationToken);
        var response = ApiResponse<List<PermissionDto>>.FromResult(result);
        return StatusCode(response.StatusCode, response);
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
        var result = await _permissionService.GetGroupedPermissionsAsync(cancellationToken);
        var response = ApiResponse<Dictionary<string, List<PermissionDto>>>.FromResult(result);
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
        var result = await _permissionService.GetUserPermissionsAsync(userId, cancellationToken);
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
        var result = await _permissionService.GetRolePermissionsAsync(roleId, cancellationToken);
        var response = ApiResponse<List<string>>.FromResult(result);
        return StatusCode(response.StatusCode, response);
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
        var result = await _permissionService.AssignPermissionsToRoleAsync(request, cancellationToken);
        var response = ApiResponse.FromResult(result);
        return StatusCode(response.StatusCode, response);
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
        {
            var errorResult = ApiResponse.BadRequest("At least one permission must be specified");
            return StatusCode(errorResult.StatusCode, errorResult);
        }

        var result = await _permissionService.RevokePermissionsFromRoleAsync(roleId, permissions, cancellationToken);
        var response = ApiResponse.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }
}

