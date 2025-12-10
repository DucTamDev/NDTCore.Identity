using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Contracts.Interfaces.Repositories;

/// <summary>
/// Repository interface for role claims management
/// </summary>
public interface IRoleClaimRepository
{
    /// <summary>
    /// Gets a role claim by its ID
    /// </summary>
    Task<AppRoleClaim?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all claims for a specific role
    /// </summary>
    Task<List<AppRoleClaim>> GetClaimsByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific claim for a role by claim type and value
    /// </summary>
    Task<AppRoleClaim?> GetRoleClaimAsync(Guid roleId, string claimType, string claimValue, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a role has a specific claim
    /// </summary>
    Task<bool> RoleHasClaimAsync(Guid roleId, string claimType, string claimValue, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all claims in the system with optional filtering
    /// </summary>
    Task<List<AppRoleClaim>> GetAllClaimsAsync(string? claimType = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new claim to a role
    /// </summary>
    Task<AppRoleClaim> AddAsync(AppRoleClaim roleClaim, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds multiple claims to a role
    /// </summary>
    Task AddRangeAsync(List<AppRoleClaim> roleClaims, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a claim from a role
    /// </summary>
    Task DeleteAsync(AppRoleClaim roleClaim, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes all claims from a role
    /// </summary>
    Task DeleteAllRoleClaimsAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a role claim exists by ID
    /// </summary>
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
}
