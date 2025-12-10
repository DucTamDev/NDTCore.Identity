using MediatR;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Common.Results;
using NDTCore.Identity.Contracts.Features.UserRoles.DTOs;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Application.Features.UserRoles.Queries.GetUserRolesByUserId;

/// <summary>
/// Handler for retrieving all roles assigned to a user
/// </summary>
public class GetUserRolesByUserIdQueryHandler : IRequestHandler<GetUserRolesByUserIdQuery, Result<List<UserRoleDto>>>
{
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly ILogger<GetUserRolesByUserIdQueryHandler> _logger;

    public GetUserRolesByUserIdQueryHandler(
        IUserRoleRepository userRoleRepository,
        ILogger<GetUserRolesByUserIdQueryHandler> logger)
    {
        _userRoleRepository = userRoleRepository;
        _logger = logger;
    }

    public async Task<Result<List<UserRoleDto>>> Handle(GetUserRolesByUserIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var userRoles = await _userRoleRepository.GetUserRolesByUserIdAsync(request.UserId, cancellationToken);
            var dtos = userRoles.Select(MapToDto).ToList();

            _logger.LogInformation("Retrieved {Count} roles for user {UserId}", dtos.Count, request.UserId);
            return Result<List<UserRoleDto>>.Success(dtos, "User roles retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user roles by user ID: {UserId}", request.UserId);
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

