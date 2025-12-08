using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Domain.Entities;
using NDTCore.Identity.Domain.Exceptions;
using NDTCore.Identity.Infrastructure.Persistence.Context;

namespace NDTCore.Identity.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for user claim management
/// </summary>
public class UserClaimRepository : IUserClaimRepository
{
    private readonly IdentityDbContext _context;
    private readonly ILogger<UserClaimRepository> _logger;

    public UserClaimRepository(
        IdentityDbContext context,
        ILogger<UserClaimRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<AppUserClaim?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.UserClaims
            .Include(uc => uc.AppUser)
            .FirstOrDefaultAsync(uc => uc.Id == id, cancellationToken);
    }

    public async Task<List<AppUserClaim>> GetClaimsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserClaims
            .Include(uc => uc.AppUser)
            .Where(uc => uc.UserId == userId)
            .OrderBy(uc => uc.ClaimType)
            .ThenBy(uc => uc.ClaimValue)
            .ToListAsync(cancellationToken);
    }

    public async Task<AppUserClaim?> GetUserClaimAsync(
        Guid userId,
        string claimType,
        string claimValue,
        CancellationToken cancellationToken = default)
    {
        return await _context.UserClaims
            .Include(uc => uc.AppUser)
            .FirstOrDefaultAsync(uc =>
                uc.UserId == userId &&
                uc.ClaimType == claimType &&
                uc.ClaimValue == claimValue, cancellationToken);
    }

    public async Task<bool> UserHasClaimAsync(
        Guid userId,
        string claimType,
        string claimValue,
        CancellationToken cancellationToken = default)
    {
        return await _context.UserClaims
            .AnyAsync(uc =>
                uc.UserId == userId &&
                uc.ClaimType == claimType &&
                uc.ClaimValue == claimValue, cancellationToken);
    }

    public async Task<List<AppUserClaim>> GetAllClaimsAsync(
        string? claimType = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.UserClaims.Include(uc => uc.AppUser).AsQueryable();

        if (!string.IsNullOrWhiteSpace(claimType))
            query = query.Where(uc => uc.ClaimType == claimType);

        return await query
            .OrderBy(uc => uc.UserId)
            .ThenBy(uc => uc.ClaimType)
            .ThenBy(uc => uc.ClaimValue)
            .ToListAsync(cancellationToken);
    }

    public async Task<AppUserClaim> AddAsync(AppUserClaim userClaim, CancellationToken cancellationToken = default)
    {
        // Check if claim already exists
        var exists = await UserHasClaimAsync(
            userClaim.UserId,
            userClaim.ClaimType!,
            userClaim.ClaimValue!,
            cancellationToken);

        if (exists)
            throw new ConflictException("This claim already exists for the user");

        await _context.UserClaims.AddAsync(userClaim, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "User claim added: UserId={UserId}, ClaimType={ClaimType}={ClaimValue}",
            userClaim.UserId, userClaim.ClaimType, userClaim.ClaimValue);

        return userClaim;
    }

    public async Task AddRangeAsync(List<AppUserClaim> userClaims, CancellationToken cancellationToken = default)
    {
        // Remove duplicates
        var distinctClaims = userClaims
            .GroupBy(uc => new { uc.UserId, uc.ClaimType, uc.ClaimValue })
            .Select(g => g.First())
            .ToList();

        // Check which claims already exist
        var newClaims = new List<AppUserClaim>();
        foreach (var claim in distinctClaims)
        {
            var exists = await UserHasClaimAsync(
                claim.UserId,
                claim.ClaimType!,
                claim.ClaimValue!,
                cancellationToken);

            if (!exists)
                newClaims.Add(claim);
        }

        if (newClaims.Any())
        {
            await _context.UserClaims.AddRangeAsync(newClaims, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Added {Count} user claims", newClaims.Count);
        }
    }

    public async Task<AppUserClaim> UpdateAsync(AppUserClaim userClaim, CancellationToken cancellationToken = default)
    {
        _context.UserClaims.Update(userClaim);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "User claim updated: ClaimId={ClaimId}, UserId={UserId}, ClaimType={ClaimType}={ClaimValue}",
            userClaim.Id, userClaim.UserId, userClaim.ClaimType, userClaim.ClaimValue);

        return userClaim;
    }

    public async Task DeleteAsync(AppUserClaim userClaim, CancellationToken cancellationToken = default)
    {
        _context.UserClaims.Remove(userClaim);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "User claim deleted: ClaimId={ClaimId}, UserId={UserId}, ClaimType={ClaimType}={ClaimValue}",
            userClaim.Id, userClaim.UserId, userClaim.ClaimType, userClaim.ClaimValue);
    }

    public async Task DeleteAllUserClaimsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var claims = await _context.UserClaims
            .Where(uc => uc.UserId == userId)
            .ToListAsync(cancellationToken);

        _context.UserClaims.RemoveRange(claims);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Deleted all claims for user: {UserId}. Count: {Count}", userId, claims.Count);
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.UserClaims
            .AnyAsync(uc => uc.Id == id, cancellationToken);
    }
}
