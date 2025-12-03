using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Common;
using NDTCore.Identity.Contracts.Features.Users.DTOs;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Domain.Entities;
using NDTCore.Identity.Infrastructure.Persistence.Context;

namespace NDTCore.Identity.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for user management
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly IdentityDbContext _context;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(
        IdentityDbContext context,
        ILogger<UserRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<AppUser>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.AppUserRoles)
                .ThenInclude(ur => ur.AppRole)
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted, cancellationToken);

            return user != null
                ? Result<AppUser>.Success(user)
                : Result<AppUser>.Failure("User not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by ID: {UserId}", id);
            return Result<AppUser>.Failure($"Error retrieving user: {ex.Message}");
        }
    }

    public async Task<Result<AppUser>> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.AppUserRoles)
                .ThenInclude(ur => ur.AppRole)
                .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted, cancellationToken);

            return user != null
                ? Result<AppUser>.Success(user)
                : Result<AppUser>.Failure("User not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by email: {Email}", email);
            return Result<AppUser>.Failure($"Error retrieving user: {ex.Message}");
        }
    }

    public async Task<Result<AppUser>> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.AppUserRoles)
                .ThenInclude(ur => ur.AppRole)
                .FirstOrDefaultAsync(u => u.UserName == userName && !u.IsDeleted, cancellationToken);

            return user != null
                ? Result<AppUser>.Success(user)
                : Result<AppUser>.Failure("User not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by username: {UserName}", userName);
            return Result<AppUser>.Failure($"Error retrieving user: {ex.Message}");
        }
    }

    public async Task<Result<PagedResult<AppUser>>> GetAllAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        bool includeDeleted = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _context.Users.AsQueryable();

            if (!includeDeleted)
                query = query.Where(u => !u.IsDeleted);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var lowerSearch = searchTerm.ToLower();
                query = query.Where(u =>
                    u.FirstName.ToLower().Contains(lowerSearch) ||
                    u.LastName.ToLower().Contains(lowerSearch) ||
                    u.Email.ToLower().Contains(lowerSearch) ||
                    u.UserName.ToLower().Contains(lowerSearch));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var users = await query
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(u => u.AppUserRoles)
                .ThenInclude(ur => ur.AppRole)
                .ToListAsync(cancellationToken);

            var pagedResult = new PagedResult<AppUser>
            {
                Items = users,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return Result<PagedResult<AppUser>>.Success(pagedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users list");
            return Result<PagedResult<AppUser>>.Failure($"Error retrieving users: {ex.Message}");
        }
    }

    public async Task<Result<List<string>>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var roles = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Join(_context.Roles,
                    ur => ur.RoleId,
                    r => r.Id,
                    (ur, r) => r.Name)
                .Where(name => name != null)
                .ToListAsync(cancellationToken);

            return Result<List<string>>.Success(roles!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user roles for user: {UserId}", userId);
            return Result<List<string>>.Failure($"Error retrieving user roles: {ex.Message}");
        }
    }

    public async Task<Result<List<AppUser>>> GetUsersByRoleAsync(string roleName, CancellationToken cancellationToken = default)
    {
        try
        {
            var users = await _context.UserRoles
                .Where(ur => ur.AppRole.Name == roleName)
                .Select(ur => ur.AppUser)
                .Where(u => !u.IsDeleted)
                .ToListAsync(cancellationToken);

            return Result<List<AppUser>>.Success(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users by role: {RoleName}", roleName);
            return Result<List<AppUser>>.Failure($"Error retrieving users: {ex.Message}");
        }
    }

    public async Task<Result<List<AppUser>>> GetLockedUsersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var users = await _context.Users
                .Where(u => !u.IsDeleted &&
                            (u.LockoutEnd != null && u.LockoutEnd > DateTimeOffset.UtcNow))
                .ToListAsync(cancellationToken);

            return Result<List<AppUser>>.Success(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting locked users");
            return Result<List<AppUser>>.Failure($"Error retrieving locked users: {ex.Message}");
        }
    }

    public async Task<Result<List<AppUser>>> GetInactiveUsersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var users = await _context.Users
                .Where(u => !u.IsDeleted && !u.IsActive)
                .ToListAsync(cancellationToken);

            return Result<List<AppUser>>.Success(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting inactive users");
            return Result<List<AppUser>>.Failure($"Error retrieving inactive users: {ex.Message}");
        }
    }

    public async Task<Result<PagedResult<AppUser>>> SearchUsersAsync(
        string? searchTerm,
        bool? isActive,
        bool? isLocked,
        DateTime? createdAfter,
        DateTime? createdBefore,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _context.Users.Where(u => !u.IsDeleted);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var lowerSearch = searchTerm.ToLower();

                query = query.Where(u =>
                    u.FirstName.ToLower().Contains(lowerSearch) ||
                    u.LastName.ToLower().Contains(lowerSearch) ||
                    u.Email.ToLower().Contains(lowerSearch) ||
                    u.UserName.ToLower().Contains(lowerSearch));
            }

            if (isActive.HasValue)
                query = query.Where(u => u.IsActive == isActive.Value);

            if (isLocked.HasValue)
            {
                if (isLocked.Value)
                    query = query.Where(u => u.LockoutEnd != null && u.LockoutEnd > DateTimeOffset.UtcNow);
                else
                    query = query.Where(u => u.LockoutEnd == null || u.LockoutEnd <= DateTimeOffset.UtcNow);
            }

            if (createdAfter.HasValue)
                query = query.Where(u => u.CreatedAt >= createdAfter.Value);

            if (createdBefore.HasValue)
                query = query.Where(u => u.CreatedAt <= createdBefore.Value);

            var totalCount = await query.CountAsync(cancellationToken);

            var users = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(u => u.AppUserRoles)
                .ThenInclude(ur => ur.AppRole)
                .ToListAsync(cancellationToken);

            var pagedResult = new PagedResult<AppUser>
            {
                Items = users,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return Result<PagedResult<AppUser>>.Success(pagedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching users");
            return Result<PagedResult<AppUser>>.Failure($"Error searching users: {ex.Message}");
        }
    }

    public async Task<Result<bool>> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var exists = await _context.Users
                .AnyAsync(u => u.Id == id && !u.IsDeleted, cancellationToken);

            return Result<bool>.Success(exists);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking user existence: {UserId}", id);
            return Result<bool>.Failure($"Error checking user existence: {ex.Message}");
        }
    }

    public async Task<Result<bool>> EmailExistsAsync(string email, Guid? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _context.Users.Where(u => u.Email == email && !u.IsDeleted);

            if (excludeUserId.HasValue)
                query = query.Where(u => u.Id != excludeUserId.Value);

            var exists = await query.AnyAsync(cancellationToken);

            return Result<bool>.Success(exists);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking email existence: {Email}", email);
            return Result<bool>.Failure($"Error checking email existence: {ex.Message}");
        }
    }

    public async Task<Result<bool>> UserNameExistsAsync(string userName, Guid? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _context.Users.Where(u => u.UserName == userName && !u.IsDeleted);

            if (excludeUserId.HasValue)
                query = query.Where(u => u.Id != excludeUserId.Value);

            var exists = await query.AnyAsync(cancellationToken);

            return Result<bool>.Success(exists);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking username existence: {UserName}", userName);
            return Result<bool>.Failure($"Error checking username existence: {ex.Message}");
        }
    }

    public async Task<Result<AppUser>> AddAsync(AppUser user, CancellationToken cancellationToken = default)
    {
        try
        {
            user.CreatedAt = DateTime.UtcNow;
            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User created: {UserId} - {Email}", user.Id, user.Email);
            return Result<AppUser>.Success(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding user: {Email}", user.Email);
            return Result<AppUser>.Failure($"Error adding user: {ex.Message}");
        }
    }

    public async Task<Result<AppUser>> UpdateAsync(AppUser user, CancellationToken cancellationToken = default)
    {
        try
        {
            user.UpdatedAt = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User updated: {UserId} - {Email}", user.Id, user.Email);
            return Result<AppUser>.Success(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user: {UserId}", user.Id);
            return Result<AppUser>.Failure($"Error updating user: {ex.Message}");
        }
    }

    public async Task<Result> DeleteAsync(AppUser user, CancellationToken cancellationToken = default)
    {
        try
        {
            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;
            user.IsActive = false;

            _context.Users.Update(user);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User soft deleted: {UserId} - {Email}", user.Id, user.Email);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user: {UserId}", user.Id);
            return Result.Failure($"Error deleting user: {ex.Message}");
        }
    }

    public async Task<Result<UserStatisticsDto>> GetUserStatisticsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var now = DateTime.UtcNow;
            var today = now.Date;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            var statistics = new UserStatisticsDto
            {
                TotalUsers = await _context.Users.CountAsync(u => !u.IsDeleted, cancellationToken),
                ActiveUsers = await _context.Users.CountAsync(u => !u.IsDeleted && u.IsActive, cancellationToken),
                LockedUsers = await _context.Users.CountAsync(u => !u.IsDeleted && u.LockoutEnd != null && u.LockoutEnd > DateTimeOffset.UtcNow, cancellationToken),
                DeletedUsers = await _context.Users.CountAsync(u => u.IsDeleted, cancellationToken),
                UsersCreatedToday = await _context.Users.CountAsync(u => !u.IsDeleted && u.CreatedAt >= today, cancellationToken),
                UsersCreatedThisMonth = await _context.Users.CountAsync(u => !u.IsDeleted && u.CreatedAt >= startOfMonth, cancellationToken)
            };

            return Result<UserStatisticsDto>.Success(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user statistics");
            return Result<UserStatisticsDto>.Failure($"Error retrieving statistics: {ex.Message}");
        }
    }
}