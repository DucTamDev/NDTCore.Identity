using NDTCore.Identity.Contracts.Common;
using System.Net;
using System.Text.Json;

namespace NDTCore.Identity.API.Middleware;

/// <summary>
/// Middleware for handling global exceptions
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of the ExceptionHandlingMiddleware class
    /// </summary>
    /// <param name="next">The next middleware in the pipeline</param>
    /// <param name="logger">The logger</param>
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = HttpStatusCode.InternalServerError;
        string? message;
        List<string>? errors = null;

        switch (exception)
        {
            case UnauthorizedAccessException:
                statusCode = HttpStatusCode.Unauthorized;
                message = exception.Message;
                break;

            case KeyNotFoundException:
                statusCode = HttpStatusCode.NotFound;
                message = exception.Message;
                break;

            case InvalidOperationException:
            case ArgumentException:
                statusCode = HttpStatusCode.BadRequest;
                message = exception.Message;
                break;

            default:
                message = exception.Message;
                break;
        }

        var response = ApiResponse<object>.ErrorResponse(
            message,
            errors,
            (int)statusCode);

        response.TraceId = context.TraceIdentifier;

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}

/// <summary>
/// Extension methods for exception handling middleware
/// </summary>
public static class ExceptionHandlingMiddlewareExtensions
{
    /// <summary>
    /// Adds exception handling middleware to the pipeline
    /// </summary>
    /// <param name="builder">The application builder</param>
    /// <returns>The application builder for chaining</returns>
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}