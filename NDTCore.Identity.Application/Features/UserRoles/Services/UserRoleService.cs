using AutoMapper;
using Microsoft.AspNetCore.Identity;
using NDTCore.Identity.Contracts.Common;
using NDTCore.Identity.Contracts.Features.UserRoles.DTOs;
using NDTCore.Identity.Contracts.Features.UserRoles.Requests;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Contracts.Interfaces.Services;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Application.Features.UserRoles.Services;

/// <summary>
/// Service for handling user-role assignment operations
/// </summary>
public class UserRoleService : IUserRoleService
{
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;

    public UserRoleService(
        IUserRoleRepository userRoleRepository,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        UserManager<AppUser> userManager,
        IMapper mapper)
    {
        _userRoleRepository = userRoleRepository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<ApiResponse<UserRoleDto>> GetUserRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        var userRoleResult = await _userRoleRepository.GetUserRoleAsync(userId, roleId, cancellationToken);
        if (userRoleResult.IsFailure)
            return ApiResponse<UserRoleDto>.FailureResponse("User role not found", 404);

        var userRole = userRoleResult.Value!;
        var dto = MapToDto(userRole);
        return ApiResponse<UserRoleDto>.SuccessResponse(dto);
    }

    public async Task<ApiResponse<List<UserRoleDto>>> GetUserRolesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var userRolesResult = await _userRoleRepository.GetUserRolesByUserIdAsync(userId, cancellationToken);
        if (userRolesResult.IsFailure)
            return ApiResponse<List<UserRoleDto>>.FailureResponse("Failed to retrieve user roles", 500);

        var userRoles = userRolesResult.Value!;
        var dtos = userRoles.Select(MapToDto).ToList();
        return ApiResponse<List<UserRoleDto>>.SuccessResponse(dtos);
    }

    public async Task<ApiResponse<List<UserRoleDto>>> GetUserRolesByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var userRolesResult = await _userRoleRepository.GetUserRolesByRoleIdAsync(roleId, cancellationToken);
        if (userRolesResult.IsFailure)
            return ApiResponse<List<UserRoleDto>>.FailureResponse("Failed to retrieve user roles", 500);

        var userRoles = userRolesResult.Value!;
        var dtos = userRoles.Select(MapToDto).ToList();
        return ApiResponse<List<UserRoleDto>>.SuccessResponse(dtos);
    }

    public async Task<ApiResponse<PagedResult<UserRoleDto>>> GetUserRolesAsync(GetUserRolesRequest request, CancellationToken cancellationToken = default)
    {
        var pagedResult = await _userRoleRepository.GetUserRolesAsync(
            request.PageNumber,
            request.PageSize,
            request.UserId,
            request.RoleId,
            cancellationToken);

        if (pagedResult.IsFailure)
            return ApiResponse<PagedResult<UserRoleDto>>.FailureResponse("Failed to retrieve user roles", 500);

        var pagedUserRoles = pagedResult.Value!;
        var dtos = pagedUserRoles.Items.Select(MapToDto).ToList();

        var result = new PagedResult<UserRoleDto>
        {
            Items = dtos,
            PageNumber = pagedUserRoles.PageNumber,
            PageSize = pagedUserRoles.PageSize,
            TotalCount = pagedUserRoles.TotalCount
        };

        return ApiResponse<PagedResult<UserRoleDto>>.SuccessResponse(result);
    }

    public async Task<ApiResponse<UserRoleDto>> AssignRoleToUserAsync(CreateUserRoleRequest request, CancellationToken cancellationToken = default)
    {
        // Validate user exists
        var userResult = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (userResult.IsFailure)
            return ApiResponse<UserRoleDto>.FailureResponse("User not found", 404);

        // Validate role exists
        var roleResult = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
        if (roleResult.IsFailure)
            return ApiResponse<UserRoleDto>.FailureResponse("Role not found", 404);

        var user = userResult.Value!;
        var role = roleResult.Value!;

        // Check if already assigned
        var existsResult = await _userRoleRepository.UserHasRoleAsync(request.UserId, request.RoleId, cancellationToken);
        if (existsResult.IsSuccess && existsResult.Value)
            return ApiResponse<UserRoleDto>.FailureResponse("User already has this role assigned", 400);

        // Use UserManager to assign role (for Identity framework consistency)
        var assignResult = await _userManager.AddToRoleAsync(user, role.Name!);
        if (!assignResult.Succeeded)
        {
            var errors = assignResult.Errors.Select(e => e.Description).ToList();
            return ApiResponse<UserRoleDto>.FailureResponse("Failed to assign role", 400, errors);
        }

        // Get the created assignment and update metadata
        var userRoleResult = await _userRoleRepository.GetUserRoleAsync(request.UserId, request.RoleId, cancellationToken);
        if (userRoleResult.IsSuccess && userRoleResult.Value != null)
        {
            var userRole = userRoleResult.Value;
            if (request.AssignedAt.HasValue)
                userRole.AssignedAt = request.AssignedAt.Value;
            if (!string.IsNullOrEmpty(request.AssignedBy))
                userRole.AssignedBy = request.AssignedBy;

            var updateResult = await _userRoleRepository.UpdateAsync(userRole, cancellationToken);
            if (updateResult.IsSuccess)
            {
                var dto = MapToDto(updateResult.Value!);
                return ApiResponse<UserRoleDto>.SuccessResponse(dto, "Role assigned successfully", 201);
            }
        }

        // Fallback: create DTO from entities
        var fallbackDto = new UserRoleDto
        {
            UserId = user.Id,
            UserName = user.UserName ?? string.Empty,
            UserEmail = user.Email ?? string.Empty,
            UserFullName = user.FullName,
            RoleId = role.Id,
            RoleName = role.Name ?? string.Empty,
            RoleDescription = role.Description,
            AssignedAt = request.AssignedAt ?? DateTime.UtcNow,
            AssignedBy = request.AssignedBy
        };

        return ApiResponse<UserRoleDto>.SuccessResponse(fallbackDto, "Role assigned successfully", 201);
    }

    public async Task<ApiResponse<UserRoleDto>> UpdateUserRoleAsync(Guid userId, Guid roleId, UpdateUserRoleRequest request, CancellationToken cancellationToken = default)
    {
        var userRoleResult = await _userRoleRepository.GetUserRoleAsync(userId, roleId, cancellationToken);
        if (userRoleResult.IsFailure)
            return ApiResponse<UserRoleDto>.FailureResponse("User role not found", 404);

        var userRole = userRoleResult.Value!;
        if (request.AssignedAt.HasValue)
            userRole.AssignedAt = request.AssignedAt.Value;
        if (!string.IsNullOrEmpty(request.AssignedBy))
            userRole.AssignedBy = request.AssignedBy;

        var updateResult = await _userRoleRepository.UpdateAsync(userRole, cancellationToken);
        if (updateResult.IsFailure)
            return ApiResponse<UserRoleDto>.FailureResponse("Failed to update user role", 500);

        var dto = MapToDto(updateResult.Value!);
        return ApiResponse<UserRoleDto>.SuccessResponse(dto, "User-role assignment updated successfully");
    }

    public async Task<ApiResponse> RemoveRoleFromUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        var userRoleResult = await _userRoleRepository.GetUserRoleAsync(userId, roleId, cancellationToken);
        if (userRoleResult.IsFailure)
            return ApiResponse.FailureResponse(message: "User role not found", statusCode: 404);

        var userResult = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (userResult.IsFailure)
            return ApiResponse.FailureResponse(message: "User not found", statusCode: 404);

        var roleResult = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
        if (roleResult.IsFailure)
            return ApiResponse.FailureResponse(message: "Role not found", statusCode: 404);

        var user = userResult.Value!;
        var role = roleResult.Value!;

        // Use UserManager to remove role (for Identity framework consistency)
        var removeResult = await _userManager.RemoveFromRoleAsync(user, role.Name!);
        if (!removeResult.Succeeded)
        {
            var errors = removeResult.Errors.Select(e => e.Description).ToList();
            return ApiResponse.FailureResponse("Failed to remove role", 400, errors);
        }

        return ApiResponse.SuccessResponse("Role removed successfully");
    }

    private UserRoleDto MapToDto(AppUserRole userRole)
    {
        return new UserRoleDto
        {
            UserId = userRole.UserId,
            UserName = userRole.AppUser?.UserName ?? string.Empty,
            UserEmail = userRole.AppUser?.Email ?? string.Empty,
            UserFullName = userRole.AppUser?.FullName ?? string.Empty,
            RoleId = userRole.RoleId,
            RoleName = userRole.AppRole?.Name ?? string.Empty,
            RoleDescription = userRole.AppRole?.Description,
            AssignedAt = userRole.AssignedAt,
            AssignedBy = userRole.AssignedBy
        };
    }
}

