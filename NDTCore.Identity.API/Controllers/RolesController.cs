using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NDTCore.Identity.API.Controllers.Base;
using NDTCore.Identity.Contracts.Common;
using NDTCore.Identity.Contracts.Common;
using NDTCore.Identity.Contracts.Features.Roles.DTOs;
using NDTCore.Identity.Contracts.Features.Roles.Requests;
using NDTCore.Identity.Contracts.Interfaces.Services;

namespace NDTCore.Identity.API.Controllers;

/// <summary>
/// Role management endpoints
/// </summary>
[ApiController]
[Route("api/roles")]
[Authorize(Policy = "AdminOnly")]
public class RolesController : BaseApiController
{
    private readonly IRoleService _roleService;
    private readonly ILogger<RolesController> _logger;

    public RolesController(
        IRoleService roleService,
        ILogger<RolesController> logger)
    {
        _roleService = roleService;
        _logger = logger;
    }

    /// <summary>
    /// Get all roles
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of roles</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<RoleDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllRoles(CancellationToken cancellationToken = default)
    {
        var response = await _roleService.GetAllRolesAsync(cancellationToken);
        
        if (!response.Success)
            return StatusCode(response.StatusCode, response);

        return Ok(response);
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
        var response = await _roleService.GetRoleByIdAsync(id, cancellationToken);
        
        if (!response.Success)
            return StatusCode(response.StatusCode, response);

        return Ok(response);
    }

    /// <summary>
    /// Create a new role
    /// </summary>
    /// <param name="request">Create role request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created role</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<RoleDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<RoleDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateRole(
        [FromBody] CreateRoleRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _roleService.CreateRoleAsync(request, cancellationToken);
        
        if (!response.Success)
            return StatusCode(response.StatusCode, response);

        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Update an existing role
    /// </summary>
    /// <param name="id">Role ID</param>
    /// <param name="request">Update role request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated role</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<RoleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<RoleDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<RoleDto>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<RoleDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateRole(
        [FromRoute] Guid id,
        [FromBody] UpdateRoleRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _roleService.UpdateRoleAsync(id, request, cancellationToken);
        
        if (!response.Success)
            return StatusCode(response.StatusCode, response);

        return Ok(response);
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
        var response = await _roleService.DeleteRoleAsync(id, cancellationToken);
        
        if (!response.Success)
            return StatusCode(response.StatusCode, response);

        return Ok(response);
    }

    /// <summary>
    /// Assign role to user
    /// </summary>
    /// <param name="request">Assign role request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpPost("assign")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignRoleToUser(
        [FromBody] AssignRoleRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _roleService.AssignRoleToUserAsync(request, cancellationToken);
        
        if (!response.Success)
            return StatusCode(response.StatusCode, response);

        return Ok(response);
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
        var response = await _roleService.RemoveRoleFromUserAsync(userId, roleId, cancellationToken);
        
        if (!response.Success)
            return StatusCode(response.StatusCode, response);

        return Ok(response);
    }
}

