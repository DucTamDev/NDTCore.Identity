using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Common.Results;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Domain.Constants;
using NDTCore.Identity.Domain.Entities;
using NDTCore.Identity.Domain.Exceptions;

namespace NDTCore.Identity.Application.Features.UserRoles.Commands.RemoveRoleFromUser;

/// <summary>
/// Handler for removing a role from a user
/// </summary>
public class RemoveRoleFromUserCommandHandler : IRequestHandler<RemoveRoleFromUserCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<RemoveRoleFromUserCommandHandler> _logger;

    public RemoveRoleFromUserCommandHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository,
        UserManager<AppUser> userManager,
        ILogger<RemoveRoleFromUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result> Handle(RemoveRoleFromUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate user exists
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
                return Result.NotFound($"User with ID '{request.UserId}' was not found");

            // Validate role exists
            var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);
            if (role == null)
                return Result.NotFound($"Role with ID '{request.RoleId}' was not found");

            // Check if user has the role
            var hasRole = await _userRoleRepository.UserHasRoleAsync(request.UserId, request.RoleId, cancellationToken);
            if (!hasRole)
                return Result.NotFound("User does not have this role assigned");

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

            _logger.LogInformation("Successfully removed role {RoleId} from user {UserId}", request.RoleId, request.UserId);
            return Result.Success("Role removed from user successfully");
        }
        catch (ConflictException ex)
        {
            _logger.LogWarning(ex, "Conflict removing role {RoleId} from user {UserId}", request.RoleId, request.UserId);
            return Result.Conflict(ex.Message);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain error removing role {RoleId} from user {UserId}", request.RoleId, request.UserId);
            return Result.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing role {RoleId} from user {UserId}", request.RoleId, request.UserId);
            return Result.InternalError("An error occurred while removing the role");
        }
    }
}

