using NDTCore.Identity.Contracts.Common.Pagination;
using NDTCore.Identity.Contracts.Common.Results;
using NDTCore.Identity.Contracts.Features.Users.DTOs;
using NDTCore.Identity.Contracts.Features.Users.Requests;

namespace NDTCore.Identity.Contracts.Interfaces.Services;

public interface IUserService
{
    Task<Result<UserDto>> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<PaginatedCollection<UserDto>>> GetUsersAsync(GetUsersRequest request, CancellationToken cancellationToken = default);
    Task<Result<UserDto>> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task<Result<UserDto>> UpdateUserAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteUserAsync(Guid id, CancellationToken cancellationToken = default);
}

