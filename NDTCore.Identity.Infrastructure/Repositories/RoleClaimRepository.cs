using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Domain.Entities;
using NDTCore.Identity.Domain.Exceptions;
using NDTCore.Identity.Infrastructure.Persistence.Context;

namespace NDTCore.Identity.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for role claims management
/// </summary>
public class RoleClaimRepository : IRoleClaimRepository
{
    private readonly IdentityDbContext _context;
    private readonly ILogger<RoleClaimRepository> _logger;

    public RoleClaimRepository(
        IdentityDbContext context,
        ILogger<RoleClaimRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<AppRoleClaim?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.RoleClaims
            .Include(rc => rc.AppRole)
            .FirstOrDefaultAsync(rc => rc.Id == id, cancellationToken);
    }

    public async Task<List<AppRoleClaim>> GetClaimsByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return await _context.RoleClaims
            .Where(rc => rc.RoleId == roleId)
            .OrderBy(rc => rc.ClaimType)
            .ThenBy(rc => rc.ClaimValue)
            .ToListAsync(cancellationToken);
    }

    public async Task<AppRoleClaim?> GetRoleClaimAsync(
        Guid roleId,
        string claimType,
        string claimValue,
        CancellationToken cancellationToken = default)
    {
        return await _context.RoleClaims
            .FirstOrDefaultAsync(rc =>
                rc.RoleId == roleId &&
                rc.ClaimType == claimType &&
                rc.ClaimValue == claimValue, cancellationToken);
    }

    public async Task<bool> RoleHasClaimAsync(
        Guid roleId,
        string claimType,
        string claimValue,
        CancellationToken cancellationToken = default)
    {
        return await _context.RoleClaims
            .AnyAsync(rc =>
                rc.RoleId == roleId &&
                rc.ClaimType == claimType &&
                rc.ClaimValue == claimValue, cancellationToken);
    }

    public async Task<List<AppRoleClaim>> GetAllClaimsAsync(
        string? claimType = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.RoleClaims.Include(rc => rc.AppRole).AsQueryable();

        if (!string.IsNullOrWhiteSpace(claimType))
            query = query.Where(rc => rc.ClaimType == claimType);

        return await query
            .OrderBy(rc => rc.AppRole.Name)
            .ThenBy(rc => rc.ClaimType)
            .ThenBy(rc => rc.ClaimValue)
            .ToListAsync(cancellationToken);
    }

    public async Task<AppRoleClaim> AddAsync(AppRoleClaim roleClaim, CancellationToken cancellationToken = default)
    {
        // Check if claim already exists
        var exists = await RoleHasClaimAsync(
            roleClaim.RoleId,
            roleClaim.ClaimType!,
            roleClaim.ClaimValue!,
            cancellationToken);

        if (exists)
            throw new ConflictException("This claim already exists for the role");

        await _context.RoleClaims.AddAsync(roleClaim, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Role claim added: {RoleId}, {ClaimType}={ClaimValue}",
            roleClaim.RoleId, roleClaim.ClaimType, roleClaim.ClaimValue);

        return roleClaim;
    }

    public async Task AddRangeAsync(List<AppRoleClaim> roleClaims, CancellationToken cancellationToken = default)
    {
        // Remove duplicates
        var distinctClaims = roleClaims
            .GroupBy(rc => new { rc.RoleId, rc.ClaimType, rc.ClaimValue })
            .Select(g => g.First())
            .ToList();

        // Check which claims already exist
        var newClaims = new List<AppRoleClaim>();
        foreach (var claim in distinctClaims)
        {
            var exists = await RoleHasClaimAsync(
                claim.RoleId,
                claim.ClaimType!,
                claim.ClaimValue!,
                cancellationToken);

            if (!exists)
                newClaims.Add(claim);
        }

        if (newClaims.Any())
        {
            await _context.RoleClaims.AddRangeAsync(newClaims, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Added {Count} role claims", newClaims.Count);
        }
    }

    public async Task DeleteAsync(AppRoleClaim roleClaim, CancellationToken cancellationToken = default)
    {
        _context.RoleClaims.Remove(roleClaim);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Role claim deleted: {RoleId}, {ClaimType}={ClaimValue}",
            roleClaim.RoleId, roleClaim.ClaimType, roleClaim.ClaimValue);
    }

    public async Task DeleteAllRoleClaimsAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var claims = await _context.RoleClaims
            .Where(rc => rc.RoleId == roleId)
            .ToListAsync(cancellationToken);

        _context.RoleClaims.RemoveRange(claims);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Deleted all claims for role: {RoleId}. Count: {Count}", roleId, claims.Count);
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.RoleClaims
            .AnyAsync(rc => rc.Id == id, cancellationToken);
    }
}
