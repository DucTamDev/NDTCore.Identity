using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Domain.Constants;
using NDTCore.Identity.Domain.Entities;
using NDTCore.Identity.Domain.Exceptions;
using NDTCore.Identity.Infrastructure.Persistence.Context;

namespace NDTCore.Identity.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for refresh token management
/// </summary>
public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly IdentityDbContext _context;
    private readonly ILogger<RefreshTokenRepository> _logger;

    public RefreshTokenRepository(
        IdentityDbContext context,
        ILogger<RefreshTokenRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<AppUserRefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _context.Set<AppUserRefreshToken>()
            .Include(rt => rt.AppUser)
            .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);
    }

    public async Task<AppUserRefreshToken?> GetByJwtIdAsync(string jwtId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<AppUserRefreshToken>()
            .Include(rt => rt.AppUser)
            .FirstOrDefaultAsync(rt => rt.JwtId == jwtId, cancellationToken);
    }

    public async Task<List<AppUserRefreshToken>> GetActiveTokensByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _context.Set<AppUserRefreshToken>()
            .Where(rt => rt.UserId == userId &&
                         !rt.IsRevoked &&
                         rt.ExpiresAt > now)
            .OrderByDescending(rt => rt.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<AppUserRefreshToken>> GetAllTokensByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<AppUserRefreshToken>()
            .Where(rt => rt.UserId == userId)
            .OrderByDescending(rt => rt.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<AppUserRefreshToken>> GetExpiredTokensAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _context.Set<AppUserRefreshToken>()
            .Where(rt => !rt.IsRevoked && rt.ExpiresAt <= now)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<AppUserRefreshToken>> GetInactiveTokensAsync(int inactiveDays, CancellationToken cancellationToken = default)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-inactiveDays);
        return await _context.Set<AppUserRefreshToken>()
            .Where(rt => !rt.IsRevoked && rt.CreatedAt <= cutoffDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<AppUserRefreshToken> AddAsync(AppUserRefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        await _context.Set<AppUserRefreshToken>().AddAsync(refreshToken, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("[{ClassName}.{FunctionName}] | Refresh token created for user: {UserId}",
            nameof(RefreshTokenRepository),
            nameof(AddAsync),
            refreshToken.UserId);

        return refreshToken;
    }

    public async Task<AppUserRefreshToken> UpdateAsync(AppUserRefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        _context.Set<AppUserRefreshToken>().Update(refreshToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Refresh token updated: {TokenId}", refreshToken.Id);
        return refreshToken;
    }

    public async Task RevokeTokenAsync(string token, string revokedByIp, CancellationToken cancellationToken = default)
    {
        var refreshToken = await _context.Set<AppUserRefreshToken>()
            .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);

        if (refreshToken == null)
            throw new NotFoundException(nameof(AppUserRefreshToken), token);

        refreshToken.IsRevoked = true;
        refreshToken.RevokedAt = DateTime.UtcNow;
        refreshToken.RevokedByIp = revokedByIp;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Refresh token revoked for user: {UserId}", refreshToken.UserId);
    }

    public async Task RevokeAllUserTokensAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var activeTokens = await _context.Set<AppUserRefreshToken>()
            .Where(rt => rt.UserId == userId && !rt.IsRevoked)
            .ToListAsync(cancellationToken);

        foreach (var token in activeTokens)
        {
            token.IsRevoked = true;
            token.RevokedAt = now;
            token.RevokedByIp = SystemConstants.SystemOperations.System;
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("All refresh tokens revoked for user: {UserId}. Count: {Count}", userId, activeTokens.Count);
    }

    public async Task RevokeTokenChainAsync(string token, string revokedByIp, CancellationToken cancellationToken = default)
    {
        var refreshToken = await _context.Set<AppUserRefreshToken>()
            .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);

        if (refreshToken == null)
            throw new NotFoundException(nameof(AppUserRefreshToken), token);

        // Revoke the current token
        refreshToken.IsRevoked = true;
        refreshToken.RevokedAt = DateTime.UtcNow;
        refreshToken.RevokedByIp = revokedByIp;

        // Find and revoke all tokens in the chain
        var chainTokens = await _context.Set<AppUserRefreshToken>()
            .Where(rt => rt.UserId == refreshToken.UserId &&
                         !rt.IsRevoked &&
                         (rt.ReplacedByToken == token || rt.Token == refreshToken.ReplacedByToken))
            .ToListAsync(cancellationToken);

        foreach (var chainToken in chainTokens)
        {
            chainToken.IsRevoked = true;
            chainToken.RevokedAt = DateTime.UtcNow;
            chainToken.RevokedByIp = revokedByIp;
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogWarning("Token chain revoked for user: {UserId}. Chain length: {Count}",
            refreshToken.UserId, chainTokens.Count + 1);
    }

    public async Task<int> DeleteExpiredTokensAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var expiredTokens = await _context.Set<AppUserRefreshToken>()
            .Where(rt => rt.ExpiresAt <= now)
            .ToListAsync(cancellationToken);

        _context.Set<AppUserRefreshToken>().RemoveRange(expiredTokens);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Deleted {Count} expired tokens", expiredTokens.Count);
        return expiredTokens.Count;
    }

    public async Task<int> DeleteOldRevokedTokensAsync(int olderThanDays, CancellationToken cancellationToken = default)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-olderThanDays);
        var oldTokens = await _context.Set<AppUserRefreshToken>()
            .Where(rt => rt.IsRevoked && rt.RevokedAt <= cutoffDate)
            .ToListAsync(cancellationToken);

        _context.Set<AppUserRefreshToken>().RemoveRange(oldTokens);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Deleted {Count} old revoked tokens", oldTokens.Count);
        return oldTokens.Count;
    }

    public async Task<bool> IsTokenValidAsync(string token, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _context.Set<AppUserRefreshToken>()
            .AnyAsync(rt => rt.Token == token &&
                            !rt.IsRevoked &&
                            rt.ExpiresAt > now, cancellationToken);
    }

    public async Task<int> GetActiveTokenCountAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _context.Set<AppUserRefreshToken>()
            .CountAsync(rt => rt.UserId == userId &&
                             !rt.IsRevoked &&
                             rt.ExpiresAt > now, cancellationToken);
    }
}
