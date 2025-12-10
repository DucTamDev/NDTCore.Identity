using NDTCore.Identity.API.Middleware;

namespace NDTCore.Identity.API.Extensions
{
    public static class WebApplicationExtensions
    {
        public static IApplicationBuilder UseWebApplicationExtensions(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<RequestLoggingMiddleware>();
            builder.UseMiddleware<ExceptionHandlingMiddleware>();

            return builder;
        }
    }
}
