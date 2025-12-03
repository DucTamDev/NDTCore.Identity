using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Common;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Domain.Entities;
using NDTCore.Identity.Infrastructure.Persistence.Context;

namespace NDTCore.Identity.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for user-role assignment management
/// </summary>
public class UserRoleRepository : IUserRoleRepository
{
    private readonly IdentityDbContext _context;
    private readonly ILogger<UserRoleRepository> _logger;

    public UserRoleRepository(
        IdentityDbContext context,
        ILogger<UserRoleRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<AppUserRole>> GetUserRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            var userRole = await _context.UserRoles
                .Include(ur => ur.AppUser)
                .Include(ur => ur.AppRole)
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);

            return userRole != null
                ? Result<AppUserRole>.Success(userRole)
                : Result<AppUserRole>.Failure("User-role assignment not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user-role assignment: UserId={UserId}, RoleId={RoleId}", userId, roleId);
            return Result<AppUserRole>.Failure($"Error retrieving user-role assignment: {ex.Message}");
        }
    }

    public async Task<Result<List<AppUserRole>>> GetUserRolesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var userRoles = await _context.UserRoles
                .Include(ur => ur.AppUser)
                .Include(ur => ur.AppRole)
                .Where(ur => ur.UserId == userId)
                .OrderBy(ur => ur.AssignedAt)
                .ToListAsync(cancellationToken);

            return Result<List<AppUserRole>>.Success(userRoles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user roles for user: {UserId}", userId);
            return Result<List<AppUserRole>>.Failure($"Error retrieving user roles: {ex.Message}");
        }
    }

    public async Task<Result<List<AppUserRole>>> GetUserRolesByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            var userRoles = await _context.UserRoles
                .Include(ur => ur.AppUser)
                .Include(ur => ur.AppRole)
                .Where(ur => ur.RoleId == roleId)
                .OrderBy(ur => ur.AssignedAt)
                .ToListAsync(cancellationToken);

            return Result<List<AppUserRole>>.Success(userRoles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users for role: {RoleId}", roleId);
            return Result<List<AppUserRole>>.Failure($"Error retrieving users for role: {ex.Message}");
        }
    }

    public async Task<Result<PagedResult<AppUserRole>>> GetUserRolesAsync(
        int pageNumber,
        int pageSize,
        Guid? userId = null,
        Guid? roleId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _context.UserRoles
                .Include(ur => ur.AppUser)
                .Include(ur => ur.AppRole)
                .AsQueryable();

            if (userId.HasValue)
                query = query.Where(ur => ur.UserId == userId.Value);

            if (roleId.HasValue)
                query = query.Where(ur => ur.RoleId == roleId.Value);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(ur => ur.AssignedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var pagedResult = new PagedResult<AppUserRole>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return Result<PagedResult<AppUserRole>>.Success(pagedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paginated user roles");
            return Result<PagedResult<AppUserRole>>.Failure($"Error retrieving user roles: {ex.Message}");
        }
    }

    public async Task<Result<bool>> UserHasRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            var exists = await _context.UserRoles
                .AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);

            return Result<bool>.Success(exists);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user has role: UserId={UserId}, RoleId={RoleId}", userId, roleId);
            return Result<bool>.Failure($"Error checking user role: {ex.Message}");
        }
    }

    public async Task<Result<AppUserRole>> AddAsync(AppUserRole userRole, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if assignment already exists
            var exists = await UserHasRoleAsync(userRole.UserId, userRole.RoleId, cancellationToken);
            if (exists.IsSuccess && exists.Value)
                return Result<AppUserRole>.Failure("User already has this role assigned");

            if (userRole.AssignedAt == default)
                userRole.AssignedAt = DateTime.UtcNow;

            await _context.UserRoles.AddAsync(userRole, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "User-role assignment added: UserId={UserId}, RoleId={RoleId}, AssignedBy={AssignedBy}",
                userRole.UserId, userRole.RoleId, userRole.AssignedBy);

            return Result<AppUserRole>.Success(userRole);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding user-role assignment");
            return Result<AppUserRole>.Failure($"Error adding user-role assignment: {ex.Message}");
        }
    }

    public async Task<Result<AppUserRole>> UpdateAsync(AppUserRole userRole, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.UserRoles.Update(userRole);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "User-role assignment updated: UserId={UserId}, RoleId={RoleId}",
                userRole.UserId, userRole.RoleId);

            return Result<AppUserRole>.Success(userRole);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user-role assignment");
            return Result<AppUserRole>.Failure($"Error updating user-role assignment: {ex.Message}");
        }
    }

    public async Task<Result> RemoveAsync(AppUserRole userRole, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "User-role assignment removed: UserId={UserId}, RoleId={RoleId}",
                userRole.UserId, userRole.RoleId);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing user-role assignment");
            return Result.Failure($"Error removing user-role assignment: {ex.Message}");
        }
    }

    public async Task<Result> RemoveAllUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var userRoles = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .ToListAsync(cancellationToken);

            _context.UserRoles.RemoveRange(userRoles);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Removed all roles for user: {UserId}. Count: {Count}", userId, userRoles.Count);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing all roles for user: {UserId}", userId);
            return Result.Failure($"Error removing user roles: {ex.Message}");
        }
    }

    public async Task<Result> RemoveAllRoleUsersAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            var userRoles = await _context.UserRoles
                .Where(ur => ur.RoleId == roleId)
                .ToListAsync(cancellationToken);

            _context.UserRoles.RemoveRange(userRoles);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Removed all users from role: {RoleId}. Count: {Count}", roleId, userRoles.Count);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing all users from role: {RoleId}", roleId);
            return Result.Failure($"Error removing role users: {ex.Message}");
        }
    }
}

