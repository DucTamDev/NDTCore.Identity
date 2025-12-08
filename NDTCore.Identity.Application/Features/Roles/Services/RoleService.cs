using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Common.Results;
using NDTCore.Identity.Contracts.Features.Roles.DTOs;
using NDTCore.Identity.Contracts.Features.Roles.Requests;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Contracts.Interfaces.Services;
using NDTCore.Identity.Domain.Constants;
using NDTCore.Identity.Domain.Entities;
using NDTCore.Identity.Domain.Exceptions;

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
    private readonly ILogger<RoleService> _logger;

    public RoleService(
        RoleManager<AppRole> roleManager,
        UserManager<AppUser> userManager,
        IRoleRepository roleRepository,
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<RoleService> logger)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _roleRepository = roleRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<RoleDto>> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var role = await _roleRepository.GetByIdAsync(id, cancellationToken);
            if (role == null)
                return Result<RoleDto>.NotFound($"Role with ID '{id}' was not found");

            var roleDto = _mapper.Map<RoleDto>(role);
            return Result<RoleDto>.Success(roleDto, "Role retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting role by ID: {RoleId}", id);
            return Result<RoleDto>.InternalError("An error occurred while retrieving the role");
        }
    }

    public async Task<Result<List<RoleDto>>> GetAllRolesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var roles = await _roleRepository.GetAllAsync(includeSystemRoles: true, cancellationToken);
            var roleDtos = _mapper.Map<List<RoleDto>>(roles);
            return Result<List<RoleDto>>.Success(roleDtos, "Roles retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all roles");
            return Result<List<RoleDto>>.InternalError("An error occurred while retrieving roles");
        }
    }

    public async Task<Result<RoleDto>> CreateRoleAsync(CreateRoleRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var existingRole = await _roleManager.FindByNameAsync(request.Name);
            if (existingRole != null)
                return Result<RoleDto>.Conflict("Role already exists");

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
                var validationErrors = result.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.Description).ToList());

                return Result<RoleDto>.BadRequest(
                    message: "One or more validation errors occurred",
                    errorCode: ErrorCodes.ValidationError,
                    validationErrors: validationErrors);
            }

            var roleDto = _mapper.Map<RoleDto>(role);
            return Result<RoleDto>.Created(roleDto, "Role created successfully");
        }
        catch (ConflictException ex)
        {
            return Result<RoleDto>.Conflict(ex.Message);
        }
        catch (DomainException ex)
        {
            return Result<RoleDto>.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating role");
            return Result<RoleDto>.InternalError("An error occurred while creating the role");
        }
    }

    public async Task<Result<RoleDto>> UpdateRoleAsync(Guid id, UpdateRoleRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var role = await _roleRepository.GetByIdAsync(id, cancellationToken);
            if (role == null)
                return Result<RoleDto>.NotFound($"Role with ID '{id}' was not found");

            if (role.IsSystemRole)
                return Result<RoleDto>.Forbidden("Cannot modify system role");

            role.Name = request.Name;
            role.Description = request.Description;
            role.Priority = request.Priority;
            role.UpdatedAt = DateTime.UtcNow;

            var result = await _roleManager.UpdateAsync(role);

            if (!result.Succeeded)
            {
                var validationErrors = result.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.Description).ToList());

                return Result<RoleDto>.BadRequest(
                    message: "One or more validation errors occurred",
                    errorCode: ErrorCodes.ValidationError,
                    validationErrors: validationErrors);
            }

            var roleDto = _mapper.Map<RoleDto>(role);
            return Result<RoleDto>.Success(roleDto, "Role updated successfully");
        }
        catch (ConflictException ex)
        {
            return Result<RoleDto>.Conflict(ex.Message);
        }
        catch (DomainException ex)
        {
            return Result<RoleDto>.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating role: {RoleId}", id);
            return Result<RoleDto>.InternalError("An error occurred while updating the role");
        }
    }

    public async Task<Result> DeleteRoleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var role = await _roleRepository.GetByIdAsync(id, cancellationToken);
            if (role == null)
                return Result.NotFound($"Role with ID '{id}' was not found");

            if (role.IsSystemRole)
                return Result.Forbidden("Cannot delete system role");

            var result = await _roleManager.DeleteAsync(role);

            if (!result.Succeeded)
            {
                var validationErrors = result.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.Description).ToList());

                return Result.BadRequest(
                    message: "One or more validation errors occurred",
                    errorCode: ErrorCodes.ValidationError,
                    validationErrors: validationErrors);
            }

            return Result.Success("Role deleted successfully");
        }
        catch (ConflictException ex)
        {
            return Result.Conflict(ex.Message);
        }
        catch (DomainException ex)
        {
            return Result.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting role: {RoleId}", id);
            return Result.InternalError("An error occurred while deleting the role");
        }
    }

    public async Task<Result> AssignRoleToUserAsync(AssignRoleRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
                return Result.NotFound($"User with ID '{request.UserId}' was not found");

            var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
            if (role == null)
                return Result.NotFound($"Role with ID '{request.RoleId}' was not found");

            var result = await _userManager.AddToRoleAsync(user, role.Name!);

            if (!result.Succeeded)
            {
                var validationErrors = result.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.Description).ToList());

                return Result.BadRequest(
                    message: "One or more validation errors occurred",
                    errorCode: ErrorCodes.ValidationError,
                    validationErrors: validationErrors);
            }

            return Result.Success("Role assigned successfully");
        }
        catch (ConflictException ex)
        {
            return Result.Conflict(ex.Message);
        }
        catch (DomainException ex)
        {
            return Result.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning role to user");
            return Result.InternalError("An error occurred while assigning the role");
        }
    }

    public async Task<Result> RemoveRoleFromUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
                return Result.NotFound($"User with ID '{userId}' was not found");

            var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
            if (role == null)
                return Result.NotFound($"Role with ID '{roleId}' was not found");

            var result = await _userManager.RemoveFromRoleAsync(user, role.Name!);

            if (!result.Succeeded)
            {
                var validationErrors = result.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.Description).ToList());

                return Result.BadRequest(
                    message: "One or more validation errors occurred",
                    errorCode: ErrorCodes.ValidationError,
                    validationErrors: validationErrors);
            }

            return Result.Success("Role removed successfully");
        }
        catch (ConflictException ex)
        {
            return Result.Conflict(ex.Message);
        }
        catch (DomainException ex)
        {
            return Result.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing role from user");
            return Result.InternalError("An error occurred while removing the role");
        }
    }
}
