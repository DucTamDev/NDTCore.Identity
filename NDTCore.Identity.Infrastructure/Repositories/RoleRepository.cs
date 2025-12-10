using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Common.Pagination;
using NDTCore.Identity.Contracts.Features.Roles.DTOs;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Domain.Entities;
using NDTCore.Identity.Infrastructure.Persistence.Context;

namespace NDTCore.Identity.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for role management
/// </summary>
public class RoleRepository : IRoleRepository
{
    private readonly IdentityDbContext _context;
    private readonly ILogger<RoleRepository> _logger;

    public RoleRepository(
        IdentityDbContext context,
        ILogger<RoleRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<AppRole?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<AppRole?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .FirstOrDefaultAsync(r => r.Name == name, cancellationToken);
    }

    public async Task<List<AppRole>> GetAllAsync(bool includeSystemRoles = true, CancellationToken cancellationToken = default)
    {
        var query = _context.Roles.AsQueryable();

        if (!includeSystemRoles)
            query = query.Where(r => !r.IsSystemRole);

        return await query
            .OrderBy(r => r.Priority)
            .ThenBy(r => r.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<PaginatedCollection<AppRole>> GetAllPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Roles.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerSearch = searchTerm.ToLower();
            query = query.Where(r =>
                (r.Name != null && r.Name.ToLower().Contains(lowerSearch)) ||
                (r.Description != null && r.Description.ToLower().Contains(lowerSearch)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var roles = await query
            .OrderBy(r => r.Priority)
            .ThenBy(r => r.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var paginationMetadata = new PaginationMetadata(currentPage: pageNumber, pageSize: pageSize, totalRecords: totalCount);

        return new PaginatedCollection<AppRole>(items: roles, pagination: paginationMetadata);
    }

    public async Task<List<AppRole>> GetSystemRolesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .Where(r => r.IsSystemRole)
            .OrderBy(r => r.Priority)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<AppRole>> GetCustomRolesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .Where(r => !r.IsSystemRole)
            .OrderBy(r => r.Priority)
            .ThenBy(r => r.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<AppRole?> GetRoleWithClaimsAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .Include(r => r.AppRoleClaims)
            .FirstOrDefaultAsync(r => r.Id == roleId, cancellationToken);
    }

    public async Task<int> GetUserCountInRoleAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return await _context.UserRoles
            .CountAsync(ur => ur.RoleId == roleId, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .AnyAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<bool> RoleNameExistsAsync(string name, Guid? excludeRoleId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Roles.Where(r => r.Name == name);

        if (excludeRoleId.HasValue)
            query = query.Where(r => r.Id != excludeRoleId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> IsSystemRoleAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var role = await _context.Roles
            .FirstOrDefaultAsync(r => r.Id == roleId, cancellationToken);

        return role?.IsSystemRole ?? false;
    }

    public async Task<AppRole> AddAsync(AppRole role, CancellationToken cancellationToken = default)
    {
        role.CreatedAt = DateTime.UtcNow;
        await _context.Roles.AddAsync(role, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Role created: {RoleId} - {RoleName}", role.Id, role.Name);
        return role;
    }

    public async Task<AppRole> UpdateAsync(AppRole role, CancellationToken cancellationToken = default)
    {
        role.UpdatedAt = DateTime.UtcNow;
        _context.Roles.Update(role);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Role updated: {RoleId} - {RoleName}", role.Id, role.Name);
        return role;
    }

    public async Task DeleteAsync(AppRole role, CancellationToken cancellationToken = default)
    {
        if (role.IsSystemRole)
            throw new Domain.Exceptions.DomainException("Cannot delete system role");

        var userCount = await GetUserCountInRoleAsync(role.Id, cancellationToken);
        if (userCount > 0)
            throw new Domain.Exceptions.DomainException($"Cannot delete role. {userCount} users are assigned to this role");

        _context.Roles.Remove(role);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Role deleted: {RoleId} - {RoleName}", role.Id, role.Name);
    }

    public async Task<RoleStatisticsDto> GetRoleStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var totalRoles = await _context.Roles.CountAsync(cancellationToken);
        var systemRoles = await _context.Roles.CountAsync(r => r.IsSystemRole, cancellationToken);

        var userCountByRole = await _context.Roles
            .Select(r => new
            {
                RoleName = r.Name,
                UserCount = _context.UserRoles.Count(ur => ur.RoleId == r.Id)
            })
            .ToDictionaryAsync(x => x.RoleName ?? "Unknown", x => x.UserCount, cancellationToken);

        return new RoleStatisticsDto
        {
            TotalRoles = totalRoles,
            SystemRoles = systemRoles,
            CustomRoles = totalRoles - systemRoles,
            UserCountByRole = userCountByRole
        };
    }
}
