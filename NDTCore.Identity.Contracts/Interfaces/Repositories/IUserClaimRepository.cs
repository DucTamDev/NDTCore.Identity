using NDTCore.Identity.Contracts.Common;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Contracts.Interfaces.Repositories;

/// <summary>
/// Repository interface for user claim management
/// </summary>
public interface IUserClaimRepository
{
    /// <summary>
    /// Gets a user claim by its ID
    /// </summary>
    Task<Result<AppUserClaim>> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all claims for a specific user
    /// </summary>
    Task<Result<List<AppUserClaim>>> GetClaimsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific claim for a user by claim type and value
    /// </summary>
    Task<Result<AppUserClaim>> GetUserClaimAsync(Guid userId, string claimType, string claimValue, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user has a specific claim
    /// </summary>
    Task<Result<bool>> UserHasClaimAsync(Guid userId, string claimType, string claimValue, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all user claims in the system with optional filtering
    /// </summary>
    Task<Result<List<AppUserClaim>>> GetAllClaimsAsync(string? claimType = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new claim to a user
    /// </summary>
    Task<Result<AppUserClaim>> AddAsync(AppUserClaim userClaim, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds multiple claims to a user
    /// </summary>
    Task<Result> AddRangeAsync(List<AppUserClaim> userClaims, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a user claim
    /// </summary>
    Task<Result<AppUserClaim>> UpdateAsync(AppUserClaim userClaim, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a claim from a user
    /// </summary>
    Task<Result> DeleteAsync(AppUserClaim userClaim, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes all claims from a user
    /// </summary>
    Task<Result> DeleteAllUserClaimsAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user claim exists by ID
    /// </summary>
    Task<Result<bool>> ExistsAsync(int id, CancellationToken cancellationToken = default);
}

