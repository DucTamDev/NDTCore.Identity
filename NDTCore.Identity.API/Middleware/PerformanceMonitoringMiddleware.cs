using System.Diagnostics;

namespace NDTCore.Identity.API.Middleware;

/// <summary>
/// Middleware to monitor request performance and log slow requests
/// </summary>
public class PerformanceMonitoringMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PerformanceMonitoringMiddleware> _logger;
    private const int SlowRequestThresholdMs = 3000; // 3 seconds

    public PerformanceMonitoringMiddleware(
        RequestDelegate next,
        ILogger<PerformanceMonitoringMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestPath = context.Request.Path;
        var requestMethod = context.Request.Method;

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            var elapsedMs = stopwatch.ElapsedMilliseconds;

            // Log slow requests
            if (elapsedMs > SlowRequestThresholdMs)
            {
                _logger.LogWarning(
                    "Slow request detected: {Method} {Path} took {ElapsedMs}ms (Status: {StatusCode})",
                    requestMethod,
                    requestPath,
                    elapsedMs,
                    context.Response.StatusCode);
            }
            else
            {
                _logger.LogInformation(
                    "Request completed: {Method} {Path} in {ElapsedMs}ms (Status: {StatusCode})",
                    requestMethod,
                    requestPath,
                    elapsedMs,
                    context.Response.StatusCode);
            }
        }
    }
}

/// <summary>
/// Extension method to register Performance Monitoring Middleware
/// </summary>
public static class PerformanceMonitoringMiddlewareExtensions
{
    public static IApplicationBuilder UsePerformanceMonitoring(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<PerformanceMonitoringMiddleware>();
    }
}

