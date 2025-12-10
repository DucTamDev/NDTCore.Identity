using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NDTCore.Identity.Contracts.Common.Results;
using NDTCore.Identity.Contracts.Features.Permissions.DTOs;
using NDTCore.Identity.Contracts.Interfaces.Authorization;
using NDTCore.Identity.Domain.Entities;

namespace NDTCore.Identity.Application.Features.Permissions.Queries.GetUserPermissions;

public class GetUserPermissionsQueryHandler : IRequestHandler<GetUserPermissionsQuery, Result<UserPermissionsDto>>
{
    private readonly IUserPermissionService _userPermissionService;
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<GetUserPermissionsQueryHandler> _logger;

    public GetUserPermissionsQueryHandler(
        IUserPermissionService userPermissionService,
        UserManager<AppUser> userManager,
        ILogger<GetUserPermissionsQueryHandler> logger)
    {
        _userPermissionService = userPermissionService;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result<UserPermissionsDto>> Handle(
        GetUserPermissionsQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting permissions for user: {UserId}", request.UserId);

        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
        {
            return Result<UserPermissionsDto>.Failure("USER_NOT_FOUND", "User not found");
        }

        var permissions = await _userPermissionService.GetUserPermissionsAsync(request.UserId, cancellationToken);
        var permissionNames = permissions.Select(p => p.Name).ToList();

        var roles = await _userManager.GetRolesAsync(user);

        var dto = new UserPermissionsDto
        {
            UserId = request.UserId,
            UserName = user.UserName ?? "",
            Roles = roles.ToList(),
            EffectivePermissions = permissionNames
        };

        return Result<UserPermissionsDto>.Success(dto);
    }
}

