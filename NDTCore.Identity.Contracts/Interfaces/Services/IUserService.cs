using NDTCore.Identity.Contracts.Common;
using NDTCore.Identity.Contracts.Features.Users.DTOs;
using NDTCore.Identity.Contracts.Features.Users.Requests;

namespace NDTCore.Identity.Contracts.Interfaces.Services;

public interface IUserService
{
    Task<ApiResponse<UserDto>> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApiResponse<PagedResult<UserDto>>> GetUsersAsync(GetUsersRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<UserDto>> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<UserDto>> UpdateUserAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse> DeleteUserAsync(Guid id, CancellationToken cancellationToken = default);
}

