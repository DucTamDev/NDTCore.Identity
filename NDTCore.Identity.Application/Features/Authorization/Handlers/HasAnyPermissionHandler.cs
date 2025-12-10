using Microsoft.AspNetCore.Authorization;
using NDTCore.Identity.Contracts.Authorization.Requirements;
using NDTCore.Identity.Contracts.Interfaces.Authorization;

namespace NDTCore.Identity.Application.Features.Authorization.Handlers;

/// <summary>
/// Authorization handler for HasAnyPermissionRequirement (OR logic)
/// </summary>
public sealed class HasAnyPermissionHandler : BasePermissionHandler<HasAnyPermissionRequirement>
{
    public HasAnyPermissionHandler(IUserPermissionService userPermissionService)
        : base(userPermissionService)
    {
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        HasAnyPermissionRequirement requirement)
    {
        if (!IsUserAuthenticated(context))
            return;

        var userId = GetUserId(context.User);
        if (!userId.HasValue)
            return;

        var hasAnyPermission = await UserPermissionService.HasAnyPermissionAsync(
            userId.Value,
            requirement.Permissions);

        if (hasAnyPermission)
        {
            context.Succeed(requirement);
        }
    }
}
