using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NDTCore.Identity.API.Controllers.Base;
using NDTCore.Identity.Application.Features.RoleClaims.Commands.AddRoleClaim;
using NDTCore.Identity.Application.Features.RoleClaims.Commands.RemoveRoleClaim;
using NDTCore.Identity.Application.Features.RoleClaims.Commands.UpdateRoleClaim;
using NDTCore.Identity.Application.Features.RoleClaims.Queries.GetRoleClaimById;
using NDTCore.Identity.Application.Features.RoleClaims.Queries.GetRoleClaims;
using NDTCore.Identity.Application.Features.UserClaims.Commands.AddUserClaim;
using NDTCore.Identity.Application.Features.UserClaims.Commands.RemoveUserClaim;
using NDTCore.Identity.Application.Features.UserClaims.Commands.UpdateUserClaim;
using NDTCore.Identity.Application.Features.UserClaims.Queries.GetUserClaimById;
using NDTCore.Identity.Application.Features.UserClaims.Queries.GetUserClaims;
using NDTCore.Identity.Contracts.Common.Responses;
using NDTCore.Identity.Contracts.Features.Claims.DTOs;

namespace NDTCore.Identity.API.Controllers;

/// <summary>
/// Claim management endpoints
/// </summary>
[ApiController]
[Route("api")]
[Authorize(Policy = "AdminOnly")]
public class ClaimsController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public ClaimsController(IMediator mediator)
    {
        _mediator = mediator;
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
        var query = new GetUserClaimsQuery { UserId = userId };
        var result = await _mediator.Send(query, cancellationToken);
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
        var query = new GetUserClaimByIdQuery { ClaimId = claimId };
        var result = await _mediator.Send(query, cancellationToken);
        var response = ApiResponse<UserClaimDto>.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Add a claim to a user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="command">Add user claim command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created user claim</returns>
    [HttpPost("users/{userId}/claims")]
    [ProducesResponseType(typeof(ApiResponse<UserClaimDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddUserClaim(
        [FromRoute] Guid userId,
        [FromBody] AddUserClaimCommand command,
        CancellationToken cancellationToken = default)
    {
        // Set UserId from route parameter
        var commandWithUserId = command with { UserId = userId };

        var result = await _mediator.Send(commandWithUserId, cancellationToken);
        var response = ApiResponse<UserClaimDto>.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Update a user claim
    /// </summary>
    /// <param name="claimId">Claim ID</param>
    /// <param name="command">Update user claim command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated user claim</returns>
    [HttpPut("user-claims/{claimId}")]
    [ProducesResponseType(typeof(ApiResponse<UserClaimDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUserClaim(
        [FromRoute] int claimId,
        [FromBody] UpdateUserClaimCommand command,
        CancellationToken cancellationToken = default)
    {
        // Set ClaimId from route parameter
        var commandWithId = command with { ClaimId = claimId };

        var result = await _mediator.Send(commandWithId, cancellationToken);
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
        var command = new RemoveUserClaimCommand { ClaimId = claimId };
        var result = await _mediator.Send(command, cancellationToken);
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
        var query = new GetRoleClaimsQuery { RoleId = roleId };
        var result = await _mediator.Send(query, cancellationToken);
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
        var query = new GetRoleClaimByIdQuery { ClaimId = claimId };
        var result = await _mediator.Send(query, cancellationToken);
        var response = ApiResponse<RoleClaimDto>.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Add a claim to a role
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <param name="command">Add role claim command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created role claim</returns>
    [HttpPost("roles/{roleId}/claims")]
    [ProducesResponseType(typeof(ApiResponse<RoleClaimDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddRoleClaim(
        [FromRoute] Guid roleId,
        [FromBody] AddRoleClaimCommand command,
        CancellationToken cancellationToken = default)
    {
        // Set RoleId from route parameter
        var commandWithRoleId = command with { RoleId = roleId };

        var result = await _mediator.Send(commandWithRoleId, cancellationToken);
        var response = ApiResponse<RoleClaimDto>.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }

    /// <summary>
    /// Update a role claim
    /// </summary>
    /// <param name="claimId">Claim ID</param>
    /// <param name="command">Update role claim command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated role claim</returns>
    [HttpPut("role-claims/{claimId}")]
    [ProducesResponseType(typeof(ApiResponse<RoleClaimDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateRoleClaim(
        [FromRoute] int claimId,
        [FromBody] UpdateRoleClaimCommand command,
        CancellationToken cancellationToken = default)
    {
        // Set ClaimId from route parameter
        var commandWithId = command with { ClaimId = claimId };

        var result = await _mediator.Send(commandWithId, cancellationToken);
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
        var command = new RemoveRoleClaimCommand { ClaimId = claimId };
        var result = await _mediator.Send(command, cancellationToken);
        var response = ApiResponse.FromResult(result);
        return StatusCode(response.StatusCode, response);
    }
}