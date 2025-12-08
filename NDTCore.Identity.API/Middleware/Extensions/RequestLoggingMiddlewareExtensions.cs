namespace NDTCore.Identity.API.Middleware.Extensions;

/// <summary>
/// Extension methods for request logging middleware
/// </summary>
public static class RequestLoggingMiddlewareExtensions
{
    /// <summary>
    /// Adds request logging middleware to the pipeline
    /// </summary>
    /// <param name="builder">The application builder</param>
    /// <returns>The application builder for chaining</returns>
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLoggingMiddleware>();
    }
}
