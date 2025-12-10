using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Common.Results;
using NDTCore.Identity.Contracts.Features.UserRoles.DTOs;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Domain.Constants;
using NDTCore.Identity.Domain.Entities;
using NDTCore.Identity.Domain.Exceptions;

namespace NDTCore.Identity.Application.Features.UserRoles.Commands.AssignRoleToUser;

/// <summary>
/// Handler for assigning a role to a user
/// </summary>
public class AssignRoleToUserCommandHandler : IRequestHandler<AssignRoleToUserCommand, Result<UserRoleDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<AssignRoleToUserCommandHandler> _logger;

    public AssignRoleToUserCommandHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository,
        UserManager<AppUser> userManager,
        ILogger<AssignRoleToUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result<UserRoleDto>> Handle(AssignRoleToUserCommand request, CancellationToken cancellationToken)
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

            // Get the created assignment
            var userRole = await _userRoleRepository.GetUserRoleAsync(request.UserId, request.RoleId, cancellationToken);
            if (userRole != null)
            {
                var dto = MapToDto(userRole);
                _logger.LogInformation("Successfully assigned role {RoleId} to user {UserId}", request.RoleId, request.UserId);
                return Result<UserRoleDto>.Created(dto, "Role assigned to user successfully");
            }

            // Fallback DTO
            var fallbackDto = new UserRoleDto
            {
                UserId = user.Id,
                UserName = user.UserName ?? string.Empty,
                UserEmail = user.Email ?? string.Empty,
                UserFullName = $"{user.FirstName} {user.LastName}".Trim(),
                RoleId = role.Id,
                RoleName = role.Name ?? string.Empty,
                RoleDescription = role.Description,
                AssignedAt = DateTime.UtcNow
            };

            return Result<UserRoleDto>.Created(fallbackDto, "Role assigned to user successfully");
        }
        catch (ConflictException ex)
        {
            _logger.LogWarning(ex, "Conflict assigning role {RoleId} to user {UserId}", request.RoleId, request.UserId);
            return Result<UserRoleDto>.Conflict(ex.Message);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain error assigning role {RoleId} to user {UserId}", request.RoleId, request.UserId);
            return Result<UserRoleDto>.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning role {RoleId} to user {UserId}", request.RoleId, request.UserId);
            return Result<UserRoleDto>.InternalError("An error occurred while assigning the role");
        }
    }

    private static UserRoleDto MapToDto(AppUserRole userRole)
    {
        return new UserRoleDto
        {
            UserId = userRole.UserId,
            UserName = userRole.AppUser?.UserName ?? string.Empty,
            UserEmail = userRole.AppUser?.Email ?? string.Empty,
            UserFullName = userRole.AppUser != null
                ? $"{userRole.AppUser.FirstName} {userRole.AppUser.LastName}".Trim()
                : string.Empty,
            RoleId = userRole.RoleId,
            RoleName = userRole.AppRole?.Name ?? string.Empty,
            RoleDescription = userRole.AppRole?.Description,
            AssignedAt = userRole.AppUser?.CreatedAt ?? DateTime.UtcNow
        };
    }
}

