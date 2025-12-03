using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Common;
using NDTCore.Identity.Contracts.Features.Roles.DTOs;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Domain.Entities;
using NDTCore.Identity.Infrastructure.Persistence.Context;

namespace NDTCore.Identity.Infrastructure.Repositories
{
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

        public async Task<Result<AppRole>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                var role = await _context.Roles
                    .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

                return role != null
                    ? Result<AppRole>.Success(role)
                    : Result<AppRole>.Failure("Role not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting role by ID: {RoleId}", id);
                return Result<AppRole>.Failure($"Error retrieving role: {ex.Message}");
            }
        }

        public async Task<Result<AppRole>> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            try
            {
                var role = await _context.Roles
                    .FirstOrDefaultAsync(r => r.Name == name, cancellationToken);

                return role != null
                    ? Result<AppRole>.Success(role)
                    : Result<AppRole>.Failure("Role not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting role by name: {RoleName}", name);
                return Result<AppRole>.Failure($"Error retrieving role: {ex.Message}");
            }
        }

        public async Task<Result<List<AppRole>>> GetAllAsync(bool includeSystemRoles = true, CancellationToken cancellationToken = default)
        {
            try
            {
                var query = _context.Roles.AsQueryable();

                if (!includeSystemRoles)
                    query = query.Where(r => !r.IsSystemRole);

                var roles = await query
                    .OrderBy(r => r.Priority)
                    .ThenBy(r => r.Name)
                    .ToListAsync(cancellationToken);

                return Result<List<AppRole>>.Success(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all roles");
                return Result<List<AppRole>>.Failure($"Error retrieving roles: {ex.Message}");
            }
        }

        public async Task<Result<PagedResult<AppRole>>> GetAllPagedAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm = null,
            CancellationToken cancellationToken = default)
        {
            try
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

                var pagedResult = new PagedResult<AppRole>
                {
                    Items = roles,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount
                };

                return Result<PagedResult<AppRole>>.Success(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paged roles");
                return Result<PagedResult<AppRole>>.Failure($"Error retrieving roles: {ex.Message}");
            }
        }

        public async Task<Result<List<AppRole>>> GetSystemRolesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var roles = await _context.Roles
                    .Where(r => r.IsSystemRole)
                    .OrderBy(r => r.Priority)
                    .ToListAsync(cancellationToken);

                return Result<List<AppRole>>.Success(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting system roles");
                return Result<List<AppRole>>.Failure($"Error retrieving system roles: {ex.Message}");
            }
        }

        public async Task<Result<List<AppRole>>> GetCustomRolesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var roles = await _context.Roles
                    .Where(r => !r.IsSystemRole)
                    .OrderBy(r => r.Priority)
                    .ThenBy(r => r.Name)
                    .ToListAsync(cancellationToken);

                return Result<List<AppRole>>.Success(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting custom roles");
                return Result<List<AppRole>>.Failure($"Error retrieving custom roles: {ex.Message}");
            }
        }

        public async Task<Result<AppRole>> GetRoleWithClaimsAsync(Guid roleId, CancellationToken cancellationToken = default)
        {
            try
            {
                var role = await _context.Roles
                    .Include(r => r.AppRoleClaims)
                    .FirstOrDefaultAsync(r => r.Id == roleId, cancellationToken);

                return role != null
                    ? Result<AppRole>.Success(role)
                    : Result<AppRole>.Failure("Role not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting role with claims: {RoleId}", roleId);
                return Result<AppRole>.Failure($"Error retrieving role: {ex.Message}");
            }
        }

        public async Task<Result<int>> GetUserCountInRoleAsync(Guid roleId, CancellationToken cancellationToken = default)
        {
            try
            {
                var count = await _context.UserRoles
                    .CountAsync(ur => ur.RoleId == roleId, cancellationToken);

                return Result<int>.Success(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user count for role: {RoleId}", roleId);
                return Result<int>.Failure($"Error getting user count: {ex.Message}");
            }
        }

        public async Task<Result<bool>> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                var exists = await _context.Roles
                    .AnyAsync(r => r.Id == id, cancellationToken);

                return Result<bool>.Success(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking role existence: {RoleId}", id);
                return Result<bool>.Failure($"Error checking role existence: {ex.Message}");
            }
        }

        public async Task<Result<bool>> RoleNameExistsAsync(string name, Guid? excludeRoleId = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var query = _context.Roles.Where(r => r.Name == name);

                if (excludeRoleId.HasValue)
                    query = query.Where(r => r.Id != excludeRoleId.Value);

                var exists = await query.AnyAsync(cancellationToken);

                return Result<bool>.Success(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking role name existence: {RoleName}", name);
                return Result<bool>.Failure($"Error checking role name: {ex.Message}");
            }
        }

        public async Task<Result<bool>> IsSystemRoleAsync(Guid roleId, CancellationToken cancellationToken = default)
        {
            try
            {
                var role = await _context.Roles
                    .FirstOrDefaultAsync(r => r.Id == roleId, cancellationToken);

                if (role == null)
                    return Result<bool>.Failure("Role not found");

                return Result<bool>.Success(role.IsSystemRole);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if role is system role: {RoleId}", roleId);
                return Result<bool>.Failure($"Error checking role type: {ex.Message}");
            }
        }

        public async Task<Result<AppRole>> AddAsync(AppRole role, CancellationToken cancellationToken = default)
        {
            try
            {
                role.CreatedAt = DateTime.UtcNow;
                await _context.Roles.AddAsync(role, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Role created: {RoleId} - {RoleName}", role.Id, role.Name);
                return Result<AppRole>.Success(role);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding role: {RoleName}", role.Name);
                return Result<AppRole>.Failure($"Error adding role: {ex.Message}");
            }
        }

        public async Task<Result<AppRole>> UpdateAsync(AppRole role, CancellationToken cancellationToken = default)
        {
            try
            {
                role.UpdatedAt = DateTime.UtcNow;
                _context.Roles.Update(role);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Role updated: {RoleId} - {RoleName}", role.Id, role.Name);
                return Result<AppRole>.Success(role);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role: {RoleId}", role.Id);
                return Result<AppRole>.Failure($"Error updating role: {ex.Message}");
            }
        }

        public async Task<Result> DeleteAsync(AppRole role, CancellationToken cancellationToken = default)
        {
            try
            {
                if (role.IsSystemRole)
                    return Result.Failure("Cannot delete system role");

                var userCount = await GetUserCountInRoleAsync(role.Id, cancellationToken);
                if (userCount.IsSuccess && userCount.Value > 0)
                    return Result.Failure($"Cannot delete role. {userCount.Value} users are assigned to this role");

                _context.Roles.Remove(role);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Role deleted: {RoleId} - {RoleName}", role.Id, role.Name);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting role: {RoleId}", role.Id);
                return Result.Failure($"Error deleting role: {ex.Message}");
            }
        }

        public async Task<Result<RoleStatisticsDto>> GetRoleStatisticsAsync(CancellationToken cancellationToken = default)
        {
            try
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

                var statistics = new RoleStatisticsDto
                {
                    TotalRoles = totalRoles,
                    SystemRoles = systemRoles,
                    CustomRoles = totalRoles - systemRoles,
                    UserCountByRole = userCountByRole
                };

                return Result<RoleStatisticsDto>.Success(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting role statistics");
                return Result<RoleStatisticsDto>.Failure($"Error retrieving statistics: {ex.Message}");
            }
        }
    }
}