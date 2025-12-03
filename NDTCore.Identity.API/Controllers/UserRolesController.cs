using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NDTCore.Identity.API.Controllers.Base;
using NDTCore.Identity.Contracts.Common;
using NDTCore.Identity.Contracts.Features.UserRoles.DTOs;
using NDTCore.Identity.Contracts.Features.UserRoles.Requests;
using NDTCore.Identity.Contracts.Interfaces.Services;

namespace NDTCore.Identity.API.Controllers;

/// <summary>
/// User-role assignment management endpoints
/// </summary>
[ApiController]
[Route("api/user-roles")]
[Authorize(Policy = "AdminOnly")]
public class UserRolesController : BaseApiController
{
    private readonly IUserRoleService _userRoleService;
    private readonly ILogger<UserRolesController> _logger;

    public UserRolesController(
        IUserRoleService userRoleService,
        ILogger<UserRolesController> logger)
    {
        _userRoleService = userRoleService;
        _logger = logger;
    }

    /// <summary>
    /// Get paginated list of user-role assignments
    /// </summary>
    /// <param name="request">Filter and pagination request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of user-role assignments</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<UserRoleDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserRoles(
        [FromQuery] GetUserRolesRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _userRoleService.GetUserRolesAsync(request, cancellationToken);

        if (!response.Success)
            return StatusCode(response.StatusCode, response);

        return Ok(response);
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
        var response = await _userRoleService.GetUserRolesByUserIdAsync(userId, cancellationToken);

        if (!response.Success)
            return StatusCode(response.StatusCode, response);

        return Ok(response);
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
        var response = await _userRoleService.GetUserRolesByRoleIdAsync(roleId, cancellationToken);

        if (!response.Success)
            return StatusCode(response.StatusCode, response);

        return Ok(response);
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
        var response = await _userRoleService.GetUserRoleAsync(userId, roleId, cancellationToken);

        if (!response.Success)
            return StatusCode(response.StatusCode, response);

        return Ok(response);
    }

    /// <summary>
    /// Assign a role to a user
    /// </summary>
    /// <param name="request">Assign role request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created user-role assignment</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<UserRoleDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignRoleToUser(
        [FromBody] CreateUserRoleRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _userRoleService.AssignRoleToUserAsync(request, cancellationToken);

        if (!response.Success)
            return StatusCode(response.StatusCode, response);

        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Update user-role assignment metadata
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="roleId">Role ID</param>
    /// <param name="request">Update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated user-role assignment</returns>
    [HttpPut("{userId}/{roleId}")]
    [ProducesResponseType(typeof(ApiResponse<UserRoleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUserRole(
        [FromRoute] Guid userId,
        [FromRoute] Guid roleId,
        [FromBody] UpdateUserRoleRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _userRoleService.UpdateUserRoleAsync(userId, roleId, request, cancellationToken);

        if (!response.Success)
            return StatusCode(response.StatusCode, response);

        return Ok(response);
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
        var response = await _userRoleService.RemoveRoleFromUserAsync(userId, roleId, cancellationToken);

        if (!response.Success)
            return StatusCode(response.StatusCode, response);

        return Ok(response);
    }
}

