using Microsoft.AspNetCore.Authorization;
using NDTCore.Identity.Contracts.Authorization.Requirements;
using NDTCore.Identity.Contracts.Interfaces.Services;

namespace NDTCore.Identity.Application.Features.Authorization.Handlers;

/// <summary>
/// Authorization handler for single permission requirements
/// </summary>
public sealed class PermissionAuthorizationHandler : BasePermissionHandler<PermissionRequirement>
{
    public PermissionAuthorizationHandler(IUserPermissionService userPermissionService)
        : base(userPermissionService)
    {
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (!IsUserAuthenticated(context))
            return;

        var userId = GetUserId(context.User);
        if (!userId.HasValue)
            return;

        var hasPermission = await UserPermissionService.HasPermissionAsync(
            userId.Value,
            requirement.Permission);

        if (hasPermission)
        {
            context.Succeed(requirement);
        }
    }
}

