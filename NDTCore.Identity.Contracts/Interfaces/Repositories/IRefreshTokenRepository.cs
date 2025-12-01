using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Contracts.Interfaces.Repositories;

public interface IRefreshTokenRepository
{
    Task<AppUserRefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<List<AppUserRefreshToken>> GetActiveTokensByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(AppUserRefreshToken refreshToken, CancellationToken cancellationToken = default);
    Task UpdateAsync(AppUserRefreshToken refreshToken, CancellationToken cancellationToken = default);
    Task RevokeAllUserTokensAsync(Guid userId, CancellationToken cancellationToken = default);
}
