using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NDTCore.Identity.Contracts.Common.Responses;
using NDTCore.Identity.Domain.Exceptions;

namespace NDTCore.Identity.API.Filters;

/// <summary>
/// Additional exception filtering layer for API responses
/// </summary>
public class ApiExceptionFilter : IExceptionFilter
{
    private readonly ILogger<ApiExceptionFilter> _logger;
    private readonly IHostEnvironment _environment;

    public ApiExceptionFilter(
        ILogger<ApiExceptionFilter> logger,
        IHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "Unhandled exception occurred");

        var message = GetUserFriendlyMessage(context.Exception);

        // Include stack trace in development
        if (_environment.IsDevelopment())
        {
            message += $"\n\nStack Trace:\n{context.Exception.StackTrace}";
        }

        var response = ApiResponse.Failure("UNHANDLED_EXCEPTION", message);

        context.Result = new ObjectResult(response)
        {
            StatusCode = GetStatusCode(context.Exception)
        };

        context.ExceptionHandled = true;
    }

    private static string GetUserFriendlyMessage(Exception exception)
    {
        return exception switch
        {
            NotFoundException => exception.Message,
            ValidationException => exception.Message,
            ConflictException => exception.Message,
            UnauthorizedException => "You are not authorized to perform this action",
            ForbiddenException => "Access to this resource is forbidden",
            DomainException => exception.Message,
            _ => "An unexpected error occurred. Please try again later."
        };
    }

    private static int GetStatusCode(Exception exception)
    {
        return exception switch
        {
            NotFoundException => StatusCodes.Status404NotFound,
            ValidationException => StatusCodes.Status400BadRequest,
            ConflictException => StatusCodes.Status409Conflict,
            UnauthorizedException => StatusCodes.Status401Unauthorized,
            ForbiddenException => StatusCodes.Status403Forbidden,
            DomainException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };
    }
}

