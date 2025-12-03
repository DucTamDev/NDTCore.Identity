using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using NDTCore.Identity.Contracts.Interfaces.Infrastructure;

namespace NDTCore.Identity.Infrastructure.Services;

/// <summary>
/// Service implementation for accessing current user context
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public Guid? UserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;

            if (Guid.TryParse(userIdClaim, out var userId))
                return userId;

            return null;
        }
    }

    public string? UserName => _httpContextAccessor.HttpContext?.User?.Identity?.Name
        ?? _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value
        ?? _httpContextAccessor.HttpContext?.User?.FindFirst("name")?.Value;

    public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value
        ?? _httpContextAccessor.HttpContext?.User?.FindFirst("email")?.Value;

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public string? IpAddress
    {
        get
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) return null;

            // Try to get IP from X-Forwarded-For header (for proxies/load balancers)
            var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                var ips = forwardedFor.Split(',');
                return ips[0].Trim();
            }

            // Try X-Real-IP header
            var realIp = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIp))
                return realIp;

            // Fallback to connection remote IP
            return httpContext.Connection.RemoteIpAddress?.ToString();
        }
    }

    public string? UserAgent => _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].FirstOrDefault();
}

