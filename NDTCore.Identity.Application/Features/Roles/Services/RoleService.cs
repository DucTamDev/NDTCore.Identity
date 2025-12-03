using AutoMapper;
using Microsoft.AspNetCore.Identity;
using NDTCore.Identity.Contracts.Common;
using NDTCore.Identity.Contracts.Features.Roles.DTOs;
using NDTCore.Identity.Contracts.Features.Roles.Requests;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Contracts.Interfaces.Services;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Application.Features.Roles.Services;

/// <summary>
/// Service for handling role management operations
/// </summary>
public class RoleService : IRoleService
{
    private readonly RoleManager<AppRole> _roleManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public RoleService(
        RoleManager<AppRole> roleManager,
        UserManager<AppUser> userManager,
        IRoleRepository roleRepository,
        IUserRepository userRepository,
        IMapper mapper)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _roleRepository = roleRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<RoleDto>> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var roleResult = await _roleRepository.GetByIdAsync(id, cancellationToken);
        if (roleResult.IsFailure)
            return ApiResponse<RoleDto>.FailureResponse("Role not found", 404);

        var role = roleResult.Value!;
        var roleDto = _mapper.Map<RoleDto>(role);
        return ApiResponse<RoleDto>.SuccessResponse(roleDto);
    }

    public async Task<ApiResponse<List<RoleDto>>> GetAllRolesAsync(CancellationToken cancellationToken = default)
    {
        var rolesResult = await _roleRepository.GetAllAsync(includeSystemRoles: true, cancellationToken);
        if (rolesResult.IsFailure)
            return ApiResponse<List<RoleDto>>.FailureResponse("Failed to retrieve roles", 500);

        var roles = rolesResult.Value!;
        var roleDtos = _mapper.Map<List<RoleDto>>(roles);
        return ApiResponse<List<RoleDto>>.SuccessResponse(roleDtos);
    }

    public async Task<ApiResponse<RoleDto>> CreateRoleAsync(CreateRoleRequest request, CancellationToken cancellationToken = default)
    {
        var existingRole = await _roleManager.FindByNameAsync(request.Name);
        if (existingRole != null)
            return ApiResponse<RoleDto>.FailureResponse("Role already exists", 400);

        var role = new AppRole
        {
            Name = request.Name,
            Description = request.Description,
            Priority = request.Priority,
            IsSystemRole = false
        };

        var result = await _roleManager.CreateAsync(role);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return ApiResponse<RoleDto>.FailureResponse("Role creation failed", 400, errors);
        }

        var roleDto = _mapper.Map<RoleDto>(role);
        return ApiResponse<RoleDto>.SuccessResponse(roleDto, "Role created successfully", 201);
    }

    public async Task<ApiResponse<RoleDto>> UpdateRoleAsync(Guid id, UpdateRoleRequest request, CancellationToken cancellationToken = default)
    {
        var roleResult = await _roleRepository.GetByIdAsync(id, cancellationToken);
        if (roleResult.IsFailure)
            return ApiResponse<RoleDto>.FailureResponse("Role not found", 404);

        var role = roleResult.Value!;
        if (role.IsSystemRole)
            return ApiResponse<RoleDto>.FailureResponse("Cannot modify system role", 403);

        role.Name = request.Name;
        role.Description = request.Description;
        role.Priority = request.Priority;
        role.UpdatedAt = DateTime.UtcNow;

        var result = await _roleManager.UpdateAsync(role);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return ApiResponse<RoleDto>.FailureResponse("Role update failed", 400, errors);
        }

        var roleDto = _mapper.Map<RoleDto>(role);
        return ApiResponse<RoleDto>.SuccessResponse(roleDto, "Role updated successfully");
    }

    public async Task<ApiResponse> DeleteRoleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var roleResult = await _roleRepository.GetByIdAsync(id, cancellationToken);
        if (roleResult.IsFailure)
            return ApiResponse.FailureResponse("Role not found", 404);

        var role = roleResult.Value!;
        if (role.IsSystemRole)
            return ApiResponse.FailureResponse("Cannot delete system role", 403);

        var result = await _roleManager.DeleteAsync(role);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return ApiResponse.FailureResponse("Role deletion failed", 400, errors);
        }

        return ApiResponse.SuccessResponse("Role deleted successfully");
    }

    public async Task<ApiResponse> AssignRoleToUserAsync(AssignRoleRequest request, CancellationToken cancellationToken = default)
    {
        var userResult = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (userResult.IsFailure)
            return ApiResponse.FailureResponse("User not found", 404);

        var roleResult = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
        if (roleResult.IsFailure)
            return ApiResponse.FailureResponse("Role not found", 404);

        var user = userResult.Value!;
        var role = roleResult.Value!;
        var result = await _userManager.AddToRoleAsync(user, role.Name!);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return ApiResponse.FailureResponse("Role assignment failed", 400, errors);
        }

        return ApiResponse.SuccessResponse("Role assigned successfully");
    }

    public async Task<ApiResponse> RemoveRoleFromUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        var userResult = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (userResult.IsFailure)
            return ApiResponse.FailureResponse(message: "User not found", statusCode: 404);

        var roleResult = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
        if (roleResult.IsFailure)
            return ApiResponse.FailureResponse("Role not found", 404);

        var user = userResult.Value!;
        var role = roleResult.Value!;
        var result = await _userManager.RemoveFromRoleAsync(user, role.Name!);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return ApiResponse.FailureResponse("Role removal failed", 400, errors);
        }

        return ApiResponse.SuccessResponse("Role removed successfully");
    }
}

