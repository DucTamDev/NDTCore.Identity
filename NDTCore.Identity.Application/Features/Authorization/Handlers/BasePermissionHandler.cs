using Microsoft.AspNetCore.Authorization;
using NDTCore.Identity.Contracts.Interfaces.Services;
using System.Security.Claims;

namespace NDTCore.Identity.Application.Features.Authorization.Handlers;

/// <summary>
/// Base class for permission authorization handlers to reduce code duplication
/// </summary>
public abstract class BasePermissionHandler<TRequirement> : AuthorizationHandler<TRequirement>
    where TRequirement : IAuthorizationRequirement
{
    protected readonly IUserPermissionService UserPermissionService;

    protected BasePermissionHandler(IUserPermissionService userPermissionService)
    {
        UserPermissionService = userPermissionService;
    }

    /// <summary>
    /// Extracts user ID from claims principal
    /// </summary>
    protected static Guid? GetUserId(ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(NDTCore.Identity.Domain.Constants.ClaimTypes.Subject)?.Value
                          ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId)
            ? null
            : userId;
    }

    /// <summary>
    /// Checks if the user is authenticated
    /// </summary>
    protected static bool IsUserAuthenticated(AuthorizationHandlerContext context)
    {
        return context.User.Identity?.IsAuthenticated == true;
    }
}