using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Common.Pagination;
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

    public async Task<AppUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(u => u.AppUserRoles)
            .ThenInclude(ur => ur.AppRole)
            .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted, cancellationToken);
    }

    public async Task<AppUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(u => u.AppUserRoles)
            .ThenInclude(ur => ur.AppRole)
            .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted, cancellationToken);
    }

    public async Task<AppUser?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(u => u.AppUserRoles)
            .ThenInclude(ur => ur.AppRole)
            .FirstOrDefaultAsync(u => u.UserName == userName && !u.IsDeleted, cancellationToken);
    }

    public async Task<PaginatedCollection<AppUser>> GetAllAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        bool includeDeleted = false,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Users.AsQueryable();

        if (!includeDeleted)
            query = query.Where(u => !u.IsDeleted);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(u =>
                u.FirstName.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase) ||
                u.LastName.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase) ||
                u.Email!.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase) ||
                u.UserName!.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase));
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

        var paginationMetadata = new PaginationMetadata(currentPage: pageNumber, pageSize: pageSize, totalRecords: totalCount);

        return new PaginatedCollection<AppUser>(items: users, pagination: paginationMetadata);
    }

    public async Task<List<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var roles = await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Join(_context.Roles,
                ur => ur.RoleId,
                r => r.Id,
                (ur, r) => r.Name)
            .Where(name => name != null)
            .ToListAsync(cancellationToken);

        return roles!;
    }

    public async Task<List<AppUser>> GetUsersByRoleAsync(string roleName, CancellationToken cancellationToken = default)
    {
        return await _context.UserRoles
            .Where(ur => ur.AppRole.Name == roleName)
            .Select(ur => ur.AppUser)
            .Where(u => !u.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<AppUser>> GetLockedUsersAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(u => !u.IsDeleted &&
                        (u.LockoutEnd != null && u.LockoutEnd > DateTimeOffset.UtcNow))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<AppUser>> GetInactiveUsersAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(u => !u.IsDeleted && !u.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<PaginatedCollection<AppUser>> SearchUsersAsync(
        string? searchTerm,
        bool? isActive,
        bool? isLocked,
        DateTime? createdAfter,
        DateTime? createdBefore,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Users.Where(u => !u.IsDeleted);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(u =>
                u.FirstName.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase) ||
                u.LastName.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase) ||
                u.Email!.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase) ||
                u.UserName!.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase));
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

        var paginationMetadata = new PaginationMetadata(currentPage: pageNumber, pageSize: pageSize, totalRecords: totalCount);

        return new PaginatedCollection<AppUser>(items: users, pagination: paginationMetadata);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AnyAsync(u => u.Id == id && !u.IsDeleted, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, Guid? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Users.Where(u => u.Email == email && !u.IsDeleted);

        if (excludeUserId.HasValue)
            query = query.Where(u => u.Id != excludeUserId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> UserNameExistsAsync(string userName, Guid? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Users.Where(u => u.UserName == userName && !u.IsDeleted);

        if (excludeUserId.HasValue)
            query = query.Where(u => u.Id != excludeUserId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<AppUser> AddAsync(AppUser user, CancellationToken cancellationToken = default)
    {
        user.CreatedAt = DateTime.UtcNow;
        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "[{ClassName}.{FunctionName}] - User created: UserId={UserId}, Email={Email}",
            nameof(UserRepository),
            nameof(AddAsync),
            user.Id,
            user.Email);

        return user;
    }

    public async Task<AppUser> UpdateAsync(AppUser user, CancellationToken cancellationToken = default)
    {
        user.UpdatedAt = DateTime.UtcNow;
        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "[{ClassName}.{FunctionName}] - User updated: UserId={UserId}, Email={Email}",
            nameof(UserRepository),
            nameof(UpdateAsync),
            user.Id,
            user.Email);

        return user;
    }

    public async Task DeleteAsync(AppUser user, CancellationToken cancellationToken = default)
    {
        user.IsDeleted = true;
        user.DeletedAt = DateTime.UtcNow;
        user.IsActive = false;

        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "[{ClassName}.{FunctionName}] - User soft deleted: UserId={UserId}, Email={Email}",
            nameof(UserRepository),
            nameof(DeleteAsync),
            user.Id,
            user.Email);
    }

    public async Task<UserStatisticsDto> GetUserStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var today = now.Date;
        var startOfMonth = new DateTime(year: now.Year, month: now.Month, day: 1, hour: now.Hour, minute: now.Minute, second: now.Second, kind: DateTimeKind.Utc);

        return new UserStatisticsDto
        {
            TotalUsers = await _context.Users.CountAsync(u => !u.IsDeleted, cancellationToken),
            ActiveUsers = await _context.Users.CountAsync(u => !u.IsDeleted && u.IsActive, cancellationToken),
            LockedUsers = await _context.Users.CountAsync(u => !u.IsDeleted && u.LockoutEnd != null && u.LockoutEnd > DateTimeOffset.UtcNow, cancellationToken),
            DeletedUsers = await _context.Users.CountAsync(u => u.IsDeleted, cancellationToken),
            UsersCreatedToday = await _context.Users.CountAsync(u => !u.IsDeleted && u.CreatedAt >= today, cancellationToken),
            UsersCreatedThisMonth = await _context.Users.CountAsync(u => !u.IsDeleted && u.CreatedAt >= startOfMonth, cancellationToken)
        };
    }
}