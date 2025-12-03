using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Common;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Domain.Entities;
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

    public async Task<Result<AppUserClaim>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var userClaim = await _context.UserClaims
                .Include(uc => uc.AppUser)
                .FirstOrDefaultAsync(uc => uc.Id == id, cancellationToken);

            return userClaim != null
                ? Result<AppUserClaim>.Success(userClaim)
                : Result<AppUserClaim>.Failure("User claim not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user claim by ID: {ClaimId}", id);
            return Result<AppUserClaim>.Failure($"Error retrieving user claim: {ex.Message}");
        }
    }

    public async Task<Result<List<AppUserClaim>>> GetClaimsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var claims = await _context.UserClaims
                .Include(uc => uc.AppUser)
                .Where(uc => uc.UserId == userId)
                .OrderBy(uc => uc.ClaimType)
                .ThenBy(uc => uc.ClaimValue)
                .ToListAsync(cancellationToken);

            return Result<List<AppUserClaim>>.Success(claims);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting claims for user: {UserId}", userId);
            return Result<List<AppUserClaim>>.Failure($"Error retrieving user claims: {ex.Message}");
        }
    }

    public async Task<Result<AppUserClaim>> GetUserClaimAsync(
        Guid userId,
        string claimType,
        string claimValue,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userClaim = await _context.UserClaims
                .Include(uc => uc.AppUser)
                .FirstOrDefaultAsync(uc =>
                    uc.UserId == userId &&
                    uc.ClaimType == claimType &&
                    uc.ClaimValue == claimValue, cancellationToken);

            return userClaim != null
                ? Result<AppUserClaim>.Success(userClaim)
                : Result<AppUserClaim>.Failure("User claim not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user claim: UserId={UserId}, ClaimType={ClaimType}, ClaimValue={ClaimValue}",
                userId, claimType, claimValue);
            return Result<AppUserClaim>.Failure($"Error retrieving user claim: {ex.Message}");
        }
    }

    public async Task<Result<bool>> UserHasClaimAsync(
        Guid userId,
        string claimType,
        string claimValue,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var exists = await _context.UserClaims
                .AnyAsync(uc =>
                    uc.UserId == userId &&
                    uc.ClaimType == claimType &&
                    uc.ClaimValue == claimValue, cancellationToken);

            return Result<bool>.Success(exists);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user has claim: UserId={UserId}, ClaimType={ClaimType}, ClaimValue={ClaimValue}",
                userId, claimType, claimValue);
            return Result<bool>.Failure($"Error checking claim: {ex.Message}");
        }
    }

    public async Task<Result<List<AppUserClaim>>> GetAllClaimsAsync(
        string? claimType = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _context.UserClaims.Include(uc => uc.AppUser).AsQueryable();

            if (!string.IsNullOrWhiteSpace(claimType))
                query = query.Where(uc => uc.ClaimType == claimType);

            var claims = await query
                .OrderBy(uc => uc.UserId)
                .ThenBy(uc => uc.ClaimType)
                .ThenBy(uc => uc.ClaimValue)
                .ToListAsync(cancellationToken);

            return Result<List<AppUserClaim>>.Success(claims);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all user claims");
            return Result<List<AppUserClaim>>.Failure($"Error retrieving claims: {ex.Message}");
        }
    }

    public async Task<Result<AppUserClaim>> AddAsync(AppUserClaim userClaim, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if claim already exists
            var exists = await UserHasClaimAsync(
                userClaim.UserId,
                userClaim.ClaimType!,
                userClaim.ClaimValue!,
                cancellationToken);

            if (exists.IsSuccess && exists.Value)
                return Result<AppUserClaim>.Failure("This claim already exists for the user");

            await _context.UserClaims.AddAsync(userClaim, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "User claim added: UserId={UserId}, ClaimType={ClaimType}={ClaimValue}",
                userClaim.UserId, userClaim.ClaimType, userClaim.ClaimValue);

            return Result<AppUserClaim>.Success(userClaim);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding user claim");
            return Result<AppUserClaim>.Failure($"Error adding user claim: {ex.Message}");
        }
    }

    public async Task<Result> AddRangeAsync(List<AppUserClaim> userClaims, CancellationToken cancellationToken = default)
    {
        try
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

                if (exists.IsSuccess && !exists.Value)
                    newClaims.Add(claim);
            }

            if (newClaims.Any())
            {
                await _context.UserClaims.AddRangeAsync(newClaims, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Added {Count} user claims", newClaims.Count);
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding multiple user claims");
            return Result.Failure($"Error adding user claims: {ex.Message}");
        }
    }

    public async Task<Result<AppUserClaim>> UpdateAsync(AppUserClaim userClaim, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.UserClaims.Update(userClaim);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "User claim updated: ClaimId={ClaimId}, UserId={UserId}, ClaimType={ClaimType}={ClaimValue}",
                userClaim.Id, userClaim.UserId, userClaim.ClaimType, userClaim.ClaimValue);

            return Result<AppUserClaim>.Success(userClaim);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user claim: {ClaimId}", userClaim.Id);
            return Result<AppUserClaim>.Failure($"Error updating user claim: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(AppUserClaim userClaim, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.UserClaims.Remove(userClaim);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "User claim deleted: ClaimId={ClaimId}, UserId={UserId}, ClaimType={ClaimType}={ClaimValue}",
                userClaim.Id, userClaim.UserId, userClaim.ClaimType, userClaim.ClaimValue);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user claim: {ClaimId}", userClaim.Id);
            return Result.Failure($"Error deleting user claim: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAllUserClaimsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var claims = await _context.UserClaims
                .Where(uc => uc.UserId == userId)
                .ToListAsync(cancellationToken);

            _context.UserClaims.RemoveRange(claims);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Deleted all claims for user: {UserId}. Count: {Count}", userId, claims.Count);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting all claims for user: {UserId}", userId);
            return Result.Failure($"Error deleting user claims: {ex.Message}");
        }
    }

    public async Task<Result<bool>> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var exists = await _context.UserClaims
                .AnyAsync(uc => uc.Id == id, cancellationToken);

            return Result<bool>.Success(exists);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking user claim existence: {ClaimId}", id);
            return Result<bool>.Failure($"Error checking claim existence: {ex.Message}");
        }
    }
}

