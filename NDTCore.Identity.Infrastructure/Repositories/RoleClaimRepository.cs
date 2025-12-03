using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Common;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Domain.Entities;
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

    public async Task<Result<AppRoleClaim>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var roleClaim = await _context.RoleClaims
                .Include(rc => rc.AppRole)
                .FirstOrDefaultAsync(rc => rc.Id == id, cancellationToken);

            return roleClaim != null
                ? Result<AppRoleClaim>.Success(roleClaim)
                : Result<AppRoleClaim>.Failure("Role claim not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting role claim by ID: {ClaimId}", id);
            return Result<AppRoleClaim>.Failure($"Error retrieving role claim: {ex.Message}");
        }
    }

    public async Task<Result<List<AppRoleClaim>>> GetClaimsByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            var claims = await _context.RoleClaims
                .Where(rc => rc.RoleId == roleId)
                .OrderBy(rc => rc.ClaimType)
                .ThenBy(rc => rc.ClaimValue)
                .ToListAsync(cancellationToken);

            return Result<List<AppRoleClaim>>.Success(claims);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting claims for role: {RoleId}", roleId);
            return Result<List<AppRoleClaim>>.Failure($"Error retrieving role claims: {ex.Message}");
        }
    }

    public async Task<Result<AppRoleClaim>> GetRoleClaimAsync(
        Guid roleId,
        string claimType,
        string claimValue,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var roleClaim = await _context.RoleClaims
                .FirstOrDefaultAsync(rc =>
                    rc.RoleId == roleId &&
                    rc.ClaimType == claimType &&
                    rc.ClaimValue == claimValue, cancellationToken);

            return roleClaim != null
                ? Result<AppRoleClaim>.Success(roleClaim)
                : Result<AppRoleClaim>.Failure("Role claim not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting role claim: {RoleId}, {ClaimType}, {ClaimValue}",
                roleId, claimType, claimValue);
            return Result<AppRoleClaim>.Failure($"Error retrieving role claim: {ex.Message}");
        }
    }

    public async Task<Result<bool>> RoleHasClaimAsync(
        Guid roleId,
        string claimType,
        string claimValue,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var exists = await _context.RoleClaims
                .AnyAsync(rc =>
                    rc.RoleId == roleId &&
                    rc.ClaimType == claimType &&
                    rc.ClaimValue == claimValue, cancellationToken);

            return Result<bool>.Success(exists);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if role has claim: {RoleId}, {ClaimType}, {ClaimValue}",
                roleId, claimType, claimValue);
            return Result<bool>.Failure($"Error checking claim: {ex.Message}");
        }
    }

    public async Task<Result<List<AppRoleClaim>>> GetAllClaimsAsync(
        string? claimType = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _context.RoleClaims.Include(rc => rc.AppRole).AsQueryable();

            if (!string.IsNullOrWhiteSpace(claimType))
                query = query.Where(rc => rc.ClaimType == claimType);

            var claims = await query
                .OrderBy(rc => rc.AppRole.Name)
                .ThenBy(rc => rc.ClaimType)
                .ThenBy(rc => rc.ClaimValue)
                .ToListAsync(cancellationToken);

            return Result<List<AppRoleClaim>>.Success(claims);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all claims");
            return Result<List<AppRoleClaim>>.Failure($"Error retrieving claims: {ex.Message}");
        }
    }

    public async Task<Result<AppRoleClaim>> AddAsync(AppRoleClaim roleClaim, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if claim already exists
            var exists = await RoleHasClaimAsync(
                roleClaim.RoleId,
                roleClaim.ClaimType!,
                roleClaim.ClaimValue!,
                cancellationToken);

            if (exists.IsSuccess && exists.Value)
                return Result<AppRoleClaim>.Failure("This claim already exists for the role");

            await _context.RoleClaims.AddAsync(roleClaim, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Role claim added: {RoleId}, {ClaimType}={ClaimValue}",
                roleClaim.RoleId, roleClaim.ClaimType, roleClaim.ClaimValue);

            return Result<AppRoleClaim>.Success(roleClaim);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding role claim");
            return Result<AppRoleClaim>.Failure($"Error adding role claim: {ex.Message}");
        }
    }

    public async Task<Result> AddRangeAsync(List<AppRoleClaim> roleClaims, CancellationToken cancellationToken = default)
    {
        try
        {
            // Remove duplicates
            var distinctClaims = roleClaims
                .GroupBy(rc => new { rc.RoleId, rc.ClaimType, rc.ClaimValue })
                .Select(g => g.First())
                .ToList();

            // Check which claims already exist
            var existingClaims = new List<AppRoleClaim>();
            foreach (var claim in distinctClaims)
            {
                var exists = await RoleHasClaimAsync(
                    claim.RoleId,
                    claim.ClaimType!,
                    claim.ClaimValue!,
                    cancellationToken);

                if (exists.IsSuccess && !exists.Value)
                    existingClaims.Add(claim);
            }

            if (existingClaims.Any())
            {
                await _context.RoleClaims.AddRangeAsync(existingClaims, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Added {Count} role claims", existingClaims.Count);
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding multiple role claims");
            return Result.Failure($"Error adding role claims: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(AppRoleClaim roleClaim, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.RoleClaims.Remove(roleClaim);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Role claim deleted: {RoleId}, {ClaimType}={ClaimValue}",
                roleClaim.RoleId, roleClaim.ClaimType, roleClaim.ClaimValue);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting role claim: {ClaimId}", roleClaim.Id);
            return Result.Failure($"Error deleting role claim: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAllRoleClaimsAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            var claims = await _context.RoleClaims
                .Where(rc => rc.RoleId == roleId)
                .ToListAsync(cancellationToken);

            _context.RoleClaims.RemoveRange(claims);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Deleted all claims for role: {RoleId}. Count: {Count}", roleId, claims.Count);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting all claims for role: {RoleId}", roleId);
            return Result.Failure($"Error deleting role claims: {ex.Message}");
        }
    }

    public async Task<Result<bool>> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var exists = await _context.RoleClaims
                .AnyAsync(rc => rc.Id == id, cancellationToken);

            return Result<bool>.Success(exists);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking role claim existence: {ClaimId}", id);
            return Result<bool>.Failure($"Error checking claim existence: {ex.Message}");
        }
    }
}