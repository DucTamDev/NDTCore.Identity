using MediatR;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Common.Results;
using NDTCore.Identity.Contracts.Features.UserRoles.DTOs;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Application.Features.UserRoles.Queries.GetUserRole;

/// <summary>
/// Handler for retrieving a specific user-role assignment
/// </summary>
public class GetUserRoleQueryHandler : IRequestHandler<GetUserRoleQuery, Result<UserRoleDto>>
{
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly ILogger<GetUserRoleQueryHandler> _logger;

    public GetUserRoleQueryHandler(
        IUserRoleRepository userRoleRepository,
        ILogger<GetUserRoleQueryHandler> logger)
    {
        _userRoleRepository = userRoleRepository;
        _logger = logger;
    }

    public async Task<Result<UserRoleDto>> Handle(GetUserRoleQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var userRole = await _userRoleRepository.GetUserRoleAsync(request.UserId, request.RoleId, cancellationToken);
            if (userRole == null)
                return Result<UserRoleDto>.NotFound("User-role assignment not found");

            var dto = MapToDto(userRole);
            _logger.LogInformation("Retrieved user-role assignment for UserId={UserId}, RoleId={RoleId}", request.UserId, request.RoleId);
            return Result<UserRoleDto>.Success(dto, "User role retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user role: UserId={UserId}, RoleId={RoleId}", request.UserId, request.RoleId);
            return Result<UserRoleDto>.InternalError("An error occurred while retrieving the user role");
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

