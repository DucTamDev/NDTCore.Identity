using AutoMapper;
using Microsoft.AspNetCore.Identity;
using NDTCore.Identity.Contracts.DTOs.Roles;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Contracts.Interfaces.Services;
using NDTCore.Identity.Contracts.Responses;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Application.Services;

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
        var role = await _roleRepository.GetByIdAsync(id, cancellationToken);
        if (role == null)
            return ApiResponse<RoleDto>.FailureResponse("Role not found", 404);

        var roleDto = _mapper.Map<RoleDto>(role);
        return ApiResponse<RoleDto>.SuccessResponse(roleDto);
    }

    public async Task<ApiResponse<List<RoleDto>>> GetAllRolesAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _roleRepository.GetAllAsync(cancellationToken);
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
        var role = await _roleRepository.GetByIdAsync(id, cancellationToken);
        if (role == null)
            return ApiResponse<RoleDto>.FailureResponse("Role not found", 404);

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
        var role = await _roleRepository.GetByIdAsync(id, cancellationToken);
        if (role == null)
            return ApiResponse.FailureResponse("Role not found", 404);

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
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            return ApiResponse.FailureResponse("User not found", 404);

        var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
        if (role == null)
            return ApiResponse.FailureResponse("Role not found", 404);

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
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
            return ApiResponse.FailureResponse("User not found", 404);

        var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
        if (role == null)
            return ApiResponse.FailureResponse("Role not found", 404);

        var result = await _userManager.RemoveFromRoleAsync(user, role.Name!);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return ApiResponse.FailureResponse("Role removal failed", 400, errors);
        }

        return ApiResponse.SuccessResponse("Role removed successfully");
    }
}