using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Contracts.Interfaces.Repositories;

public interface IRoleRepository
{
    Task<AppRole?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AppRole?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<List<AppRole>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(AppRole role, CancellationToken cancellationToken = default);
    Task UpdateAsync(AppRole role, CancellationToken cancellationToken = default);
    Task DeleteAsync(AppRole role, CancellationToken cancellationToken = default);
}
