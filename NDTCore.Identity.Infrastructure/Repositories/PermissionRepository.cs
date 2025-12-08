using Microsoft.EntityFrameworkCore;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Domain.Entities;
using NDTCore.Identity.Domain.Exceptions;
using NDTCore.Identity.Infrastructure.Persistence.Context;

namespace NDTCore.Identity.Infrastructure.Repositories;

public class PermissionRepository : IPermissionRepository
{
    private readonly IdentityDbContext _context;

    public PermissionRepository(IdentityDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Permission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Permissions.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Permission?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Permissions.FirstOrDefaultAsync(p => p.Name == name, cancellationToken);
    }

    public async Task<List<Permission>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Permissions
            .OrderBy(p => p.Category)
            .ThenBy(p => p.SortOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Permission>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default)
    {
        return await _context.Permissions
            .Where(p => p.Category == category)
            .OrderBy(p => p.SortOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Permission>> GetByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return await _context.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .Include(rp => rp.Permission)
            .Select(rp => rp.Permission)
            .ToListAsync(cancellationToken);
    }

    public async Task<Permission> AddAsync(Permission permission, CancellationToken cancellationToken = default)
    {
        await _context.Permissions.AddAsync(permission, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return permission;
    }

    public async Task<Permission> UpdateAsync(Permission permission, CancellationToken cancellationToken = default)
    {
        _context.Permissions.Update(permission);
        await _context.SaveChangesAsync(cancellationToken);

        return permission;
    }

    public async Task DeleteAsync(Permission permission, CancellationToken cancellationToken = default)
    {
        if (permission.IsSystemPermission)
        {
            throw new DomainException("System permissions cannot be deleted.");
        }

        _context.Permissions.Remove(permission);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
