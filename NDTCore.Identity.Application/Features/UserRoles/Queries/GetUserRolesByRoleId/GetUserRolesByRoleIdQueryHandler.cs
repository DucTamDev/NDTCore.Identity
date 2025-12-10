using MediatR;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Common.Results;
using NDTCore.Identity.Contracts.Features.UserRoles.DTOs;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Application.Features.UserRoles.Queries.GetUserRolesByRoleId;

/// <summary>
/// Handler for retrieving all users assigned to a role
/// </summary>
public class GetUserRolesByRoleIdQueryHandler : IRequestHandler<GetUserRolesByRoleIdQuery, Result<List<UserRoleDto>>>
{
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly ILogger<GetUserRolesByRoleIdQueryHandler> _logger;

    public GetUserRolesByRoleIdQueryHandler(
        IUserRoleRepository userRoleRepository,
        ILogger<GetUserRolesByRoleIdQueryHandler> logger)
    {
        _userRoleRepository = userRoleRepository;
        _logger = logger;
    }

    public async Task<Result<List<UserRoleDto>>> Handle(GetUserRolesByRoleIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var userRoles = await _userRoleRepository.GetUserRolesByRoleIdAsync(request.RoleId, cancellationToken);
            var dtos = userRoles.Select(MapToDto).ToList();

            _logger.LogInformation("Retrieved {Count} users for role {RoleId}", dtos.Count, request.RoleId);
            return Result<List<UserRoleDto>>.Success(dtos, "User roles retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user roles by role ID: {RoleId}", request.RoleId);
            return Result<List<UserRoleDto>>.InternalError("An error occurred while retrieving user roles");
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

