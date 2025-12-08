using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Contracts.Interfaces.Repositories;

public interface IPermissionRepository
{
    Task<Permission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Permission?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<List<Permission>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<Permission>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default);
    Task<List<Permission>> GetByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<Permission> AddAsync(Permission permission, CancellationToken cancellationToken = default);
    Task<Permission> UpdateAsync(Permission permission, CancellationToken cancellationToken = default);
    Task DeleteAsync(Permission permission, CancellationToken cancellationToken = default);
}
