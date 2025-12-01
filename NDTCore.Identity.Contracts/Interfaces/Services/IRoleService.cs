using NDTCore.Identity.Contracts.DTOs.Roles;
using NDTCore.Identity.Contracts.Responses;

namespace NDTCore.Identity.Contracts.Interfaces.Services;

public interface IRoleService
{
    Task<ApiResponse<RoleDto>> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiResponse<List<RoleDto>>> GetAllRolesAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<RoleDto>> CreateRoleAsync(CreateRoleRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<RoleDto>> UpdateRoleAsync(Guid id, UpdateRoleRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse> DeleteRoleAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiResponse> AssignRoleToUserAsync(AssignRoleRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse> RemoveRoleFromUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
}
