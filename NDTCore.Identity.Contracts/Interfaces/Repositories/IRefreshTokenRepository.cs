using NDTCore.Identity.Contracts.Common;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Contracts.Interfaces.Repositories;

/// <summary>
/// Repository interface for refresh token management
/// </summary>
public interface IRefreshTokenRepository
{
    /// <summary>
    /// Gets a refresh token by its token string
    /// </summary>
    Task<AppUserRefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a refresh token by JWT ID
    /// </summary>
    Task<AppUserRefreshToken?> GetByJwtIdAsync(string jwtId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active tokens for a specific user
    /// </summary>
    Task<List<AppUserRefreshToken>> GetActiveTokensByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all tokens (active and revoked) for a specific user
    /// </summary>
    Task<List<AppUserRefreshToken>> GetAllTokensByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets expired but not revoked tokens
    /// </summary>
    Task<List<AppUserRefreshToken>> GetExpiredTokensAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets inactive tokens (not used for N days)
    /// </summary>
    Task<List<AppUserRefreshToken>> GetInactiveTokensAsync(int inactiveDays, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new refresh token
    /// </summary>
    Task<AppUserRefreshToken> AddAsync(AppUserRefreshToken refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing refresh token
    /// </summary>
    Task<AppUserRefreshToken> UpdateAsync(AppUserRefreshToken refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes a specific token
    /// </summary>
    Task RevokeTokenAsync(string token, string revokedByIp, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes all tokens for a specific user
    /// </summary>
    Task RevokeAllUserTokensAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes token chain (all tokens in a rotation chain)
    /// </summary>
    Task RevokeTokenChainAsync(string token, string revokedByIp, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes expired tokens (cleanup)
    /// </summary>
    Task<int> DeleteExpiredTokensAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes revoked tokens older than N days
    /// </summary>
    Task<int> DeleteOldRevokedTokensAsync(int olderThanDays, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if token exists and is valid
    /// </summary>
    Task<bool> IsTokenValidAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets count of active tokens for a user
    /// </summary>
    Task<int> GetActiveTokenCountAsync(Guid userId, CancellationToken cancellationToken = default);
}
