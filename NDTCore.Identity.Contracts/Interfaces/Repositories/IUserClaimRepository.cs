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
    Task<AppUserClaim?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all claims for a specific user
    /// </summary>
    Task<List<AppUserClaim>> GetClaimsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific claim for a user by claim type and value
    /// </summary>
    Task<AppUserClaim?> GetUserClaimAsync(Guid userId, string claimType, string claimValue, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user has a specific claim
    /// </summary>
    Task<bool> UserHasClaimAsync(Guid userId, string claimType, string claimValue, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all user claims in the system with optional filtering
    /// </summary>
    Task<List<AppUserClaim>> GetAllClaimsAsync(string? claimType = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new claim to a user
    /// </summary>
    Task<AppUserClaim> AddAsync(AppUserClaim userClaim, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds multiple claims to a user
    /// </summary>
    Task AddRangeAsync(List<AppUserClaim> userClaims, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a user claim
    /// </summary>
    Task<AppUserClaim> UpdateAsync(AppUserClaim userClaim, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a claim from a user
    /// </summary>
    Task DeleteAsync(AppUserClaim userClaim, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes all claims from a user
    /// </summary>
    Task DeleteAllUserClaimsAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user claim exists by ID
    /// </summary>
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
}
