using NDTCore.Identity.Contracts.Common.Results;
using NDTCore.Identity.Contracts.Features.Roles.DTOs;
using NDTCore.Identity.Contracts.Features.Roles.Requests;

namespace NDTCore.Identity.Contracts.Interfaces.Services;

public interface IRoleService
{
    Task<Result<RoleDto>> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<List<RoleDto>>> GetAllRolesAsync(CancellationToken cancellationToken = default);
    Task<Result<RoleDto>> CreateRoleAsync(CreateRoleRequest request, CancellationToken cancellationToken = default);
    Task<Result<RoleDto>> UpdateRoleAsync(Guid id, UpdateRoleRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteRoleAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> AssignRoleToUserAsync(AssignRoleRequest request, CancellationToken cancellationToken = default);
    Task<Result> RemoveRoleFromUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
}
