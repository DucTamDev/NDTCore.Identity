using NDTCore.Identity.Contracts.Common.Results;
using NDTCore.Identity.Contracts.Features.Claims.DTOs;
using NDTCore.Identity.Contracts.Features.Claims.Requests;

namespace NDTCore.Identity.Contracts.Interfaces.Services;

/// <summary>
/// Service interface for claim management
/// </summary>
public interface IClaimService
{
    // User Claims
    /// <summary>
    /// Gets all claims for a user
    /// </summary>
    Task<Result<List<UserClaimDto>>> GetUserClaimsAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific user claim by ID
    /// </summary>
    Task<Result<UserClaimDto>> GetUserClaimByIdAsync(int claimId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a claim to a user
    /// </summary>
    Task<Result<UserClaimDto>> AddUserClaimAsync(Guid userId, CreateClaimRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a user claim
    /// </summary>
    Task<Result<UserClaimDto>> UpdateUserClaimAsync(int claimId, UpdateClaimRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a claim from a user
    /// </summary>
    Task<Result> RemoveUserClaimAsync(int claimId, CancellationToken cancellationToken = default);

    // Role Claims
    /// <summary>
    /// Gets all claims for a role
    /// </summary>
    Task<Result<List<RoleClaimDto>>> GetRoleClaimsAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific role claim by ID
    /// </summary>
    Task<Result<RoleClaimDto>> GetRoleClaimByIdAsync(int claimId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a claim to a role
    /// </summary>
    Task<Result<RoleClaimDto>> AddRoleClaimAsync(Guid roleId, CreateClaimRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a role claim
    /// </summary>
    Task<Result<RoleClaimDto>> UpdateRoleClaimAsync(int claimId, UpdateClaimRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a claim from a role
    /// </summary>
    Task<Result> RemoveRoleClaimAsync(int claimId, CancellationToken cancellationToken = default);
}

