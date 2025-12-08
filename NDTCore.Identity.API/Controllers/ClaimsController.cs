using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NDTCore.Identity.API.Controllers.Base;
using NDTCore.Identity.Contracts.Common;
using NDTCore.Identity.Contracts.Features.Claims.DTOs;
using NDTCore.Identity.Contracts.Features.Claims.Requests;
using NDTCore.Identity.Contracts.Interfaces.Services;

namespace NDTCore.Identity.API.Controllers;

/// <summary>
/// Claim management endpoints
/// </summary>
[ApiController]
[Route("api")]
[Authorize(Policy = "AdminOnly")]
public class ClaimsController : BaseApiController
{
    private readonly IClaimService _claimService;

    public ClaimsController(
        IClaimService claimService)
    {
        _claimService = claimService;
    }

    // User Claims Endpoints

    /// <summary>
    /// Get all claims for a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of user claims</returns>
    [HttpGet("users/{userId}/claims")]
    [ProducesResponseType(typeof(ApiResponse<List<UserClaimDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserClaims(
        [FromRoute] Guid userId,
        CancellationToken cancellationToken = default)
    {
        var result = await _claimService.GetUserClaimsAsync(userId, cancellationToken);
        var response = ApiResponse<List<UserClaimDto>>.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Get a specific user claim by ID
    /// </summary>
    /// <param name="claimId">Claim ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User claim details</returns>
    [HttpGet("user-claims/{claimId}")]
    [ProducesResponseType(typeof(ApiResponse<UserClaimDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserClaimById(
        [FromRoute] int claimId,
        CancellationToken cancellationToken = default)
    {
        var result = await _claimService.GetUserClaimByIdAsync(claimId, cancellationToken);
        var response = ApiResponse<UserClaimDto>.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Add a claim to a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="request">Create claim request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created user claim</returns>
    [HttpPost("users/{userId}/claims")]
    [ProducesResponseType(typeof(ApiResponse<UserClaimDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddUserClaim(
        [FromRoute] Guid userId,
        [FromBody] CreateClaimRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await _claimService.AddUserClaimAsync(userId, request, cancellationToken);
        var response = ApiResponse<UserClaimDto>.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Update a user claim
    /// </summary>
    /// <param name="claimId">Claim ID</param>
    /// <param name="request">Update claim request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated user claim</returns>
    [HttpPut("user-claims/{claimId}")]
    [ProducesResponseType(typeof(ApiResponse<UserClaimDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUserClaim(
        [FromRoute] int claimId,
        [FromBody] UpdateClaimRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await _claimService.UpdateUserClaimAsync(claimId, request, cancellationToken);
        var response = ApiResponse<UserClaimDto>.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Remove a claim from a user
    /// </summary>
    /// <param name="claimId">Claim ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpDelete("user-claims/{claimId}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveUserClaim(
        [FromRoute] int claimId,
        CancellationToken cancellationToken = default)
    {
        var result = await _claimService.RemoveUserClaimAsync(claimId, cancellationToken);
        var response = ApiResponse.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }

    // Role Claims Endpoints

    /// <summary>
    /// Get all claims for a role
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of role claims</returns>
    [HttpGet("roles/{roleId}/claims")]
    [ProducesResponseType(typeof(ApiResponse<List<RoleClaimDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRoleClaims(
        [FromRoute] Guid roleId,
        CancellationToken cancellationToken = default)
    {
        var result = await _claimService.GetRoleClaimsAsync(roleId, cancellationToken);
        var response = ApiResponse<List<RoleClaimDto>>.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Get a specific role claim by ID
    /// </summary>
    /// <param name="claimId">Claim ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Role claim details</returns>
    [HttpGet("role-claims/{claimId}")]
    [ProducesResponseType(typeof(ApiResponse<RoleClaimDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRoleClaimById(
        [FromRoute] int claimId,
        CancellationToken cancellationToken = default)
    {
        var result = await _claimService.GetRoleClaimByIdAsync(claimId, cancellationToken);
        var response = ApiResponse<RoleClaimDto>.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Add a claim to a role
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <param name="request">Create claim request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created role claim</returns>
    [HttpPost("roles/{roleId}/claims")]
    [ProducesResponseType(typeof(ApiResponse<RoleClaimDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddRoleClaim(
        [FromRoute] Guid roleId,
        [FromBody] CreateClaimRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await _claimService.AddRoleClaimAsync(roleId, request, cancellationToken);
        var response = ApiResponse<RoleClaimDto>.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Update a role claim
    /// </summary>
    /// <param name="claimId">Claim ID</param>
    /// <param name="request">Update claim request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated role claim</returns>
    [HttpPut("role-claims/{claimId}")]
    [ProducesResponseType(typeof(ApiResponse<RoleClaimDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateRoleClaim(
        [FromRoute] int claimId,
        [FromBody] UpdateClaimRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await _claimService.UpdateRoleClaimAsync(claimId, request, cancellationToken);
        var response = ApiResponse<RoleClaimDto>.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Remove a claim from a role
    /// </summary>
    /// <param name="claimId">Claim ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpDelete("role-claims/{claimId}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveRoleClaim(
        [FromRoute] int claimId,
        CancellationToken cancellationToken = default)
    {
        var result = await _claimService.RemoveRoleClaimAsync(claimId, cancellationToken);
        var response = ApiResponse.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }
}

