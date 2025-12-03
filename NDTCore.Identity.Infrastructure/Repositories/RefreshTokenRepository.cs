using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Common;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Domain.Entities;
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

    public async Task<Result<AppUserRefreshToken>> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        try
        {
            var refreshToken = await _context.Set<AppUserRefreshToken>()
                .Include(rt => rt.AppUser)
                .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);

            return refreshToken != null
                ? Result<AppUserRefreshToken>.Success(refreshToken)
                : Result<AppUserRefreshToken>.Failure("Refresh token not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting refresh token");
            return Result<AppUserRefreshToken>.Failure($"Error retrieving refresh token: {ex.Message}");
        }
    }

    public async Task<Result<AppUserRefreshToken>> GetByJwtIdAsync(string jwtId, CancellationToken cancellationToken = default)
    {
        try
        {
            var refreshToken = await _context.Set<AppUserRefreshToken>()
                .Include(rt => rt.AppUser)
                .FirstOrDefaultAsync(rt => rt.JwtId == jwtId, cancellationToken);

            return refreshToken != null
                ? Result<AppUserRefreshToken>.Success(refreshToken)
                : Result<AppUserRefreshToken>.Failure("Refresh token not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting refresh token by JWT ID: {JwtId}", jwtId);
            return Result<AppUserRefreshToken>.Failure($"Error retrieving refresh token: {ex.Message}");
        }
    }

    public async Task<Result<List<AppUserRefreshToken>>> GetActiveTokensByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var now = DateTime.UtcNow;
            var tokens = await _context.Set<AppUserRefreshToken>()
                .Where(rt => rt.UserId == userId &&
                             !rt.IsRevoked &&
                             rt.ExpiresAt > now)
                .OrderByDescending(rt => rt.CreatedAt)
                .ToListAsync(cancellationToken);

            return Result<List<AppUserRefreshToken>>.Success(tokens);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active tokens for user: {UserId}", userId);
            return Result<List<AppUserRefreshToken>>.Failure($"Error retrieving active tokens: {ex.Message}");
        }
    }

    public async Task<Result<List<AppUserRefreshToken>>> GetAllTokensByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var tokens = await _context.Set<AppUserRefreshToken>()
                .Where(rt => rt.UserId == userId)
                .OrderByDescending(rt => rt.CreatedAt)
                .ToListAsync(cancellationToken);

            return Result<List<AppUserRefreshToken>>.Success(tokens);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all tokens for user: {UserId}", userId);
            return Result<List<AppUserRefreshToken>>.Failure($"Error retrieving tokens: {ex.Message}");
        }
    }

    public async Task<Result<List<AppUserRefreshToken>>> GetExpiredTokensAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var now = DateTime.UtcNow;
            var tokens = await _context.Set<AppUserRefreshToken>()
                .Where(rt => !rt.IsRevoked && rt.ExpiresAt <= now)
                .ToListAsync(cancellationToken);

            return Result<List<AppUserRefreshToken>>.Success(tokens);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting expired tokens");
            return Result<List<AppUserRefreshToken>>.Failure($"Error retrieving expired tokens: {ex.Message}");
        }
    }

    public async Task<Result<List<AppUserRefreshToken>>> GetInactiveTokensAsync(int inactiveDays, CancellationToken cancellationToken = default)
    {
        try
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-inactiveDays);
            var tokens = await _context.Set<AppUserRefreshToken>()
                .Where(rt => !rt.IsRevoked && rt.CreatedAt <= cutoffDate)
                .ToListAsync(cancellationToken);

            return Result<List<AppUserRefreshToken>>.Success(tokens);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting inactive tokens");
            return Result<List<AppUserRefreshToken>>.Failure($"Error retrieving inactive tokens: {ex.Message}");
        }
    }

    public async Task<Result<AppUserRefreshToken>> AddAsync(AppUserRefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        try
        {
            refreshToken.CreatedAt = DateTime.UtcNow;
            await _context.Set<AppUserRefreshToken>().AddAsync(refreshToken, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Refresh token created for user: {UserId}", refreshToken.UserId);
            return Result<AppUserRefreshToken>.Success(refreshToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding refresh token for user: {UserId}", refreshToken.UserId);
            return Result<AppUserRefreshToken>.Failure($"Error adding refresh token: {ex.Message}");
        }
    }

    public async Task<Result<AppUserRefreshToken>> UpdateAsync(AppUserRefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Set<AppUserRefreshToken>().Update(refreshToken);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Refresh token updated: {TokenId}", refreshToken.Id);
            return Result<AppUserRefreshToken>.Success(refreshToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating refresh token: {TokenId}", refreshToken.Id);
            return Result<AppUserRefreshToken>.Failure($"Error updating refresh token: {ex.Message}");
        }
    }

    public async Task<Result> RevokeTokenAsync(string token, string revokedByIp, CancellationToken cancellationToken = default)
    {
        try
        {
            var refreshToken = await _context.Set<AppUserRefreshToken>()
                .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);

            if (refreshToken == null)
                return Result.Failure("Refresh token not found");

            refreshToken.IsRevoked = true;
            refreshToken.RevokedAt = DateTime.UtcNow;
            refreshToken.RevokedByIp = revokedByIp;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Refresh token revoked for user: {UserId}", refreshToken.UserId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking refresh token");
            return Result.Failure($"Error revoking refresh token: {ex.Message}");
        }
    }

    public async Task<Result> RevokeAllUserTokensAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var now = DateTime.UtcNow;
            var activeTokens = await _context.Set<AppUserRefreshToken>()
                .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .ToListAsync(cancellationToken);

            foreach (var token in activeTokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = now;
                token.RevokedByIp = "System";
            }

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("All refresh tokens revoked for user: {UserId}. Count: {Count}", userId, activeTokens.Count);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking all tokens for user: {UserId}", userId);
            return Result.Failure($"Error revoking tokens: {ex.Message}");
        }
    }

    public async Task<Result> RevokeTokenChainAsync(string token, string revokedByIp, CancellationToken cancellationToken = default)
    {
        try
        {
            var refreshToken = await _context.Set<AppUserRefreshToken>()
                .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);

            if (refreshToken == null)
                return Result.Failure("Refresh token not found");

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
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking token chain");
            return Result.Failure($"Error revoking token chain: {ex.Message}");
        }
    }

    public async Task<Result<int>> DeleteExpiredTokensAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var now = DateTime.UtcNow;
            var expiredTokens = await _context.Set<AppUserRefreshToken>()
                .Where(rt => rt.ExpiresAt <= now)
                .ToListAsync(cancellationToken);

            _context.Set<AppUserRefreshToken>().RemoveRange(expiredTokens);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Deleted {Count} expired tokens", expiredTokens.Count);
            return Result<int>.Success(expiredTokens.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting expired tokens");
            return Result<int>.Failure($"Error deleting expired tokens: {ex.Message}");
        }
    }

    public async Task<Result<int>> DeleteOldRevokedTokensAsync(int olderThanDays, CancellationToken cancellationToken = default)
    {
        try
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-olderThanDays);
            var oldTokens = await _context.Set<AppUserRefreshToken>()
                .Where(rt => rt.IsRevoked && rt.RevokedAt <= cutoffDate)
                .ToListAsync(cancellationToken);

            _context.Set<AppUserRefreshToken>().RemoveRange(oldTokens);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Deleted {Count} old revoked tokens", oldTokens.Count);
            return Result<int>.Success(oldTokens.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting old revoked tokens");
            return Result<int>.Failure($"Error deleting old tokens: {ex.Message}");
        }
    }

    public async Task<Result<bool>> IsTokenValidAsync(string token, CancellationToken cancellationToken = default)
    {
        try
        {
            var now = DateTime.UtcNow;
            var isValid = await _context.Set<AppUserRefreshToken>()
                .AnyAsync(rt => rt.Token == token &&
                                !rt.IsRevoked &&
                                rt.ExpiresAt > now, cancellationToken);

            return Result<bool>.Success(isValid);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating token");
            return Result<bool>.Failure($"Error validating token: {ex.Message}");
        }
    }

    public async Task<Result<int>> GetActiveTokenCountAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var now = DateTime.UtcNow;
            var count = await _context.Set<AppUserRefreshToken>()
                .CountAsync(rt => rt.UserId == userId &&
                                 !rt.IsRevoked &&
                                 rt.ExpiresAt > now, cancellationToken);

            return Result<int>.Success(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active token count for user: {UserId}", userId);
            return Result<int>.Failure($"Error getting token count: {ex.Message}");
        }
    }
}