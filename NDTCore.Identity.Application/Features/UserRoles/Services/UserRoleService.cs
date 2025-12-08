using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Common.Pagination;
using NDTCore.Identity.Contracts.Common.Results;
using NDTCore.Identity.Contracts.Features.UserRoles.DTOs;
using NDTCore.Identity.Contracts.Features.UserRoles.Requests;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Contracts.Interfaces.Services;
using NDTCore.Identity.Domain.Constants;
using NDTCore.Identity.Domain.Entities;
using NDTCore.Identity.Domain.Exceptions;

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
    private readonly ILogger<UserRoleService> _logger;

    public UserRoleService(
        IUserRoleRepository userRoleRepository,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        UserManager<AppUser> userManager,
        ILogger<UserRoleService> logger)
    {
        _userRoleRepository = userRoleRepository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result<UserRoleDto>> GetUserRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            var userRole = await _userRoleRepository.GetUserRoleAsync(userId, roleId, cancellationToken);
            if (userRole == null)
                return Result<UserRoleDto>.NotFound($"User-role assignment not found");

            var dto = MapToDto(userRole);
            return Result<UserRoleDto>.Success(dto, "User role retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user role: UserId={UserId}, RoleId={RoleId}", userId, roleId);
            return Result<UserRoleDto>.InternalError("An error occurred while retrieving the user role");
        }
    }

    public async Task<Result<List<UserRoleDto>>> GetUserRolesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var userRoles = await _userRoleRepository.GetUserRolesByUserIdAsync(userId, cancellationToken);
            var dtos = userRoles.Select(MapToDto).ToList();
            return Result<List<UserRoleDto>>.Success(dtos, "User roles retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user roles by user ID: {UserId}", userId);
            return Result<List<UserRoleDto>>.InternalError("An error occurred while retrieving user roles");
        }
    }

    public async Task<Result<List<UserRoleDto>>> GetUserRolesByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            var userRoles = await _userRoleRepository.GetUserRolesByRoleIdAsync(roleId, cancellationToken);
            var dtos = userRoles.Select(MapToDto).ToList();
            return Result<List<UserRoleDto>>.Success(dtos, "User roles retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user roles by role ID: {RoleId}", roleId);
            return Result<List<UserRoleDto>>.InternalError("An error occurred while retrieving user roles");
        }
    }

    public async Task<Result<PaginatedCollection<UserRoleDto>>> GetUserRolesAsync(GetUserRolesRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var pagedUserRoles = await _userRoleRepository.GetUserRolesAsync(
                request.PageNumber,
                request.PageSize,
                request.UserId,
                request.RoleId,
                cancellationToken);

            var dtos = pagedUserRoles.Items.Select(MapToDto).ToList();

            var result = new PaginatedCollection<UserRoleDto>(items: dtos, pagination: pagedUserRoles.Metadata);

            return Result<PaginatedCollection<UserRoleDto>>.Success(result, "User roles retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paginated user roles");
            return Result<PaginatedCollection<UserRoleDto>>.InternalError("An error occurred while retrieving user roles");
        }
    }

    public async Task<Result<UserRoleDto>> AssignRoleToUserAsync(CreateUserRoleRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate user exists
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
                return Result<UserRoleDto>.NotFound($"User with ID '{request.UserId}' was not found");

            // Validate role exists
            var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
            if (role == null)
                return Result<UserRoleDto>.NotFound($"Role with ID '{request.RoleId}' was not found");

            // Check if already assigned
            var alreadyHasRole = await _userRoleRepository.UserHasRoleAsync(request.UserId, request.RoleId, cancellationToken);
            if (alreadyHasRole)
                return Result<UserRoleDto>.Conflict("User already has this role assigned");

            // Use UserManager to assign role (for Identity framework consistency)
            var assignResult = await _userManager.AddToRoleAsync(user, role.Name!);
            if (!assignResult.Succeeded)
            {
                var validationErrors = assignResult.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.Description).ToList());

                return Result<UserRoleDto>.BadRequest(
                    "One or more validation errors occurred",
                    ErrorCodes.ValidationError,
                    validationErrors);
            }

            // Get the created assignment and update metadata
            var userRole = await _userRoleRepository.GetUserRoleAsync(request.UserId, request.RoleId, cancellationToken);
            if (userRole != null)
            {
                if (request.AssignedAt.HasValue)
                    userRole.AssignedAt = request.AssignedAt.Value;
                if (!string.IsNullOrEmpty(request.AssignedBy))
                    userRole.AssignedBy = request.AssignedBy;

                userRole = await _userRoleRepository.UpdateAsync(userRole, cancellationToken);
                if (userRole != null)
                {
                    var dto = MapToDto(userRole);
                    return Result<UserRoleDto>.Success(dto, "Role assigned successfully");
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

            return Result<UserRoleDto>.Success(fallbackDto, "Role assigned successfully");
        }
        catch (ConflictException ex)
        {
            return Result<UserRoleDto>.Conflict(ex.Message);
        }
        catch (DomainException ex)
        {
            return Result<UserRoleDto>.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning role to user");
            return Result<UserRoleDto>.InternalError("An error occurred while assigning the role");
        }
    }

    public async Task<Result<UserRoleDto>> UpdateUserRoleAsync(Guid userId, Guid roleId, UpdateUserRoleRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var userRole = await _userRoleRepository.GetUserRoleAsync(userId, roleId, cancellationToken);
            if (userRole == null)
                return Result<UserRoleDto>.NotFound("User-role assignment not found");

            if (request.AssignedAt.HasValue)
                userRole.AssignedAt = request.AssignedAt.Value;
            if (!string.IsNullOrEmpty(request.AssignedBy))
                userRole.AssignedBy = request.AssignedBy;

            var updatedUserRole = await _userRoleRepository.UpdateAsync(userRole, cancellationToken);
            if (updatedUserRole == null)
                return Result<UserRoleDto>.InternalError("Failed to update user-role assignment");

            var dto = MapToDto(updatedUserRole);
            return Result<UserRoleDto>.Success(dto, "User-role assignment updated successfully");
        }
        catch (ConflictException ex)
        {
            return Result<UserRoleDto>.Conflict(ex.Message);
        }
        catch (DomainException ex)
        {
            return Result<UserRoleDto>.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user role");
            return Result<UserRoleDto>.InternalError("An error occurred while updating the user-role assignment");
        }
    }

    public async Task<Result> RemoveRoleFromUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            var userRole = await _userRoleRepository.GetUserRoleAsync(userId, roleId, cancellationToken);
            if (userRole == null)
                return Result.NotFound("User-role assignment not found");

            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
                return Result.NotFound($"User with ID '{userId}' was not found");

            var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
            if (role == null)
                return Result.NotFound($"Role with ID '{roleId}' was not found");

            // Use UserManager to remove role (for Identity framework consistency)
            var removeResult = await _userManager.RemoveFromRoleAsync(user, role.Name!);
            if (!removeResult.Succeeded)
            {
                var validationErrors = removeResult.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.Description).ToList());

                return Result.BadRequest(
                    "One or more validation errors occurred",
                    ErrorCodes.ValidationError,
                    validationErrors);
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