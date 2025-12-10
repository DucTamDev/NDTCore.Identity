using Microsoft.AspNetCore.Mvc;

namespace NDTCore.Identity.API.Controllers.Base;

/// <summary>
/// Base controller for all API controllers
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>
    /// Gets the current authenticated user ID from claims
    /// </summary>
    /// <returns>User ID</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when user ID is invalid or missing</exception>
    protected Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst(NDTCore.Identity.Domain.Constants.ClaimTypes.Subject)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
        {
            throw new UnauthorizedAccessException("Invalid or missing user identity");
        }

        return userId;
    }

    /// <summary>
    /// Gets the client IP address from the current HTTP context
    /// </summary>
    /// <returns>Client IP address or "Unknown" if not available</returns>
    protected string GetClientIpAddress()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }
}
