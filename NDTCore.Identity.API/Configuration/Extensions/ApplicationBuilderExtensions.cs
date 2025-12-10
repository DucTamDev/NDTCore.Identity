namespace NDTCore.Identity.API.Configuration.Extensions;

/// <summary>
/// IApplicationBuilder extension methods for middleware configuration
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Configure common middleware pipeline
    /// </summary>
    public static IApplicationBuilder UseCommonMiddleware(this IApplicationBuilder app)
    {
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}

