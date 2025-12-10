using Microsoft.AspNetCore.Authorization;
using NDTCore.Identity.Contracts.Authorization.Requirements;
using NDTCore.Identity.Contracts.Interfaces.Authorization;

namespace NDTCore.Identity.Application.Features.Authorization.Handlers;

/// <summary>
/// Authorization handler for HasAllPermissionsRequirement (AND logic)
/// </summary>
public sealed class HasAllPermissionsHandler : BasePermissionHandler<HasAllPermissionsRequirement>
{
    public HasAllPermissionsHandler(IUserPermissionService userPermissionService)
        : base(userPermissionService)
    {
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        HasAllPermissionsRequirement requirement)
    {
        if (!IsUserAuthenticated(context))
            return;

        var userId = GetUserId(context.User);
        if (!userId.HasValue)
            return;

        var hasAllPermissions = await UserPermissionService.HasAllPermissionsAsync(
            userId.Value,
            requirement.Permissions);

        if (hasAllPermissions)
        {
            context.Succeed(requirement);
        }
    }
}

