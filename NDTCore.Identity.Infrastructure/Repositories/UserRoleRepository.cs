using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Common.Pagination;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Domain.Entities;
using NDTCore.Identity.Domain.Exceptions;
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

    public async Task<AppUserRole?> GetUserRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        return await _context.UserRoles
            .Include(ur => ur.AppUser)
            .Include(ur => ur.AppRole)
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);
    }

    public async Task<List<AppUserRole>> GetUserRolesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserRoles
            .Include(ur => ur.AppUser)
            .Include(ur => ur.AppRole)
            .Where(ur => ur.UserId == userId)
            .OrderBy(ur => ur.AssignedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<AppUserRole>> GetUserRolesByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return await _context.UserRoles
            .Include(ur => ur.AppUser)
            .Include(ur => ur.AppRole)
            .Where(ur => ur.RoleId == roleId)
            .OrderBy(ur => ur.AssignedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<PaginatedCollection<AppUserRole>> GetUserRolesAsync(
        int pageNumber,
        int pageSize,
        Guid? userId = null,
        Guid? roleId = null,
        CancellationToken cancellationToken = default)
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

        var paginationMetadata = new PaginationMetadata(currentPage: pageNumber, pageSize: pageSize, totalRecords: totalCount);

        return new PaginatedCollection<AppUserRole>(items: items, pagination: paginationMetadata);
    }

    public async Task<bool> UserHasRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        return await _context.UserRoles
            .AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);
    }

    public async Task<AppUserRole> AddAsync(AppUserRole userRole, CancellationToken cancellationToken = default)
    {
        // Check if assignment already exists
        var exists = await UserHasRoleAsync(userRole.UserId, userRole.RoleId, cancellationToken);
        if (exists)
            throw new ConflictException("User already has this role assigned");

        if (userRole.AssignedAt == default)
            userRole.AssignedAt = DateTime.UtcNow;

        await _context.UserRoles.AddAsync(userRole, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "User-role assignment added: UserId={UserId}, RoleId={RoleId}, AssignedBy={AssignedBy}",
            userRole.UserId, userRole.RoleId, userRole.AssignedBy);

        return userRole;
    }

    public async Task<AppUserRole> UpdateAsync(AppUserRole userRole, CancellationToken cancellationToken = default)
    {
        _context.UserRoles.Update(userRole);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "User-role assignment updated: UserId={UserId}, RoleId={RoleId}",
            userRole.UserId, userRole.RoleId);

        return userRole;
    }

    public async Task RemoveAsync(AppUserRole userRole, CancellationToken cancellationToken = default)
    {
        _context.UserRoles.Remove(userRole);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "User-role assignment removed: UserId={UserId}, RoleId={RoleId}",
            userRole.UserId, userRole.RoleId);
    }

    public async Task RemoveAllUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var userRoles = await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .ToListAsync(cancellationToken);

        _context.UserRoles.RemoveRange(userRoles);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Removed all roles for user: {UserId}. Count: {Count}", userId, userRoles.Count);
    }

    public async Task RemoveAllRoleUsersAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var userRoles = await _context.UserRoles
            .Where(ur => ur.RoleId == roleId)
            .ToListAsync(cancellationToken);

        _context.UserRoles.RemoveRange(userRoles);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Removed all users from role: {RoleId}. Count: {Count}", roleId, userRoles.Count);
    }
}
