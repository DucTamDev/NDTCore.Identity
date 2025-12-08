namespace NDTCore.Identity.API.Middleware.Extensions;

/// <summary>
/// Extension methods for global exception handling middleware
/// </summary>
public static class GlobalExceptionHandlingMiddlewareExtensions
{
    /// <summary>
    /// Adds global exception handling middleware to the pipeline
    /// </summary>
    /// <param name="builder">The application builder</param>
    /// <returns>The application builder for chaining</returns>
    public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<GlobalExceptionHandlingMiddleware>();
    }
}
