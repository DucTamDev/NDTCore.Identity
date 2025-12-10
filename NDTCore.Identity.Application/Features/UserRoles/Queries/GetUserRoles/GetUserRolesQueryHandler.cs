using MediatR;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Common.Pagination;
using NDTCore.Identity.Contracts.Common.Results;
using NDTCore.Identity.Contracts.Features.UserRoles.DTOs;
using NDTCore.Identity.Contracts.Interfaces.Repositories;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Application.Features.UserRoles.Queries.GetUserRoles;

/// <summary>
/// Handler for retrieving paginated list of user-role assignments
/// </summary>
public class GetUserRolesQueryHandler : IRequestHandler<GetUserRolesQuery, Result<PaginatedCollection<UserRoleDto>>>
{
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly ILogger<GetUserRolesQueryHandler> _logger;

    public GetUserRolesQueryHandler(
        IUserRoleRepository userRoleRepository,
        ILogger<GetUserRolesQueryHandler> logger)
    {
        _userRoleRepository = userRoleRepository;
        _logger = logger;
    }

    public async Task<Result<PaginatedCollection<UserRoleDto>>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
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

            _logger.LogInformation("Retrieved {Count} user-role assignments (page {PageNumber})", dtos.Count, request.PageNumber);
            return Result<PaginatedCollection<UserRoleDto>>.Success(result, "User roles retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paginated user roles");
            return Result<PaginatedCollection<UserRoleDto>>.InternalError("An error occurred while retrieving user roles");
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

