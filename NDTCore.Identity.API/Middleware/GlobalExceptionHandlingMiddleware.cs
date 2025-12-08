using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NDTCore.Identity.Contracts.Common;
using NDTCore.Identity.Contracts.Helpers;
using NDTCore.Identity.Domain.Constants;
using NDTCore.Identity.Domain.Exceptions;
using System.Diagnostics;
using System.Text.Json;

namespace NDTCore.Identity.API.Middleware;

/// <summary>
/// Enterprise-grade exception handling middleware with structured logging and context
/// </summary>
public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlingMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var correlationId = context.TraceIdentifier;

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            await HandleExceptionAsync(context, ex, correlationId, stopwatch.ElapsedMilliseconds);
        }
    }

    private async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception,
        string correlationId,
        long elapsedMs)
    {
        // Prevent double-writing to response if already started
        if (context.Response.HasStarted)
        {
            _logger.LogWarning(
                "[{ClassName}.{FunctionName}] - Cannot write error response; response has already started: CorrelationId={CorrelationId}",
                nameof(GlobalExceptionHandlingMiddleware),
                nameof(HandleExceptionAsync),
                correlationId);

            return;
        }

        // Set correlation ID on base domain exceptions
        if (exception is BaseDomainException baseEx)
        {
            baseEx.CorrelationId = correlationId;
        }

        var response = MapExceptionToApiResponse(exception);
        response.TraceId = correlationId;

        // Log with structured data
        LogException(exception, context, correlationId, elapsedMs, response);

        // Set response headers
        context.Response.Clear();
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = response.StatusCode;

        // Add retry-after header for rate limit exceptions
        if (exception is RateLimitExceededException rateLimitEx)
        {
            context.Response.Headers["Retry-After"] = rateLimitEx.RetryAfterSeconds.ToString();
        }

        // Add CORS headers if needed
        if (!context.Response.Headers.ContainsKey("Access-Control-Allow-Origin"))
        {
            context.Response.Headers.AccessControlAllowOrigin = "*";
        }

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = _environment.IsDevelopment(),
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        try
        {
            var json = JsonSerializer.Serialize(response, options);
            await context.Response.WriteAsync(json);
        }
        catch (Exception serializationEx)
        {
            _logger.LogError(serializationEx, "[{ClassName}.{FunctionName}] - Failed to serialize error response: Error={Error}",
                nameof(GlobalExceptionHandlingMiddleware),
                nameof(HandleExceptionAsync),
                serializationEx.Message);

            // Fallback to simple error response
            var fallbackResponse = new
            {
                success = false,
                message = "An error occurred while processing your request.",
                traceId = correlationId
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(fallbackResponse));
        }
    }

    private ApiResponse MapExceptionToApiResponse(Exception exception)
    {
        return exception switch
        {
            // Base Domain Exceptions - use ErrorCode to map to status code
            BaseDomainException baseEx => MapBaseDomainException(baseEx),

            // Framework Exceptions
            UnauthorizedAccessException => CreateErrorResponse(
                ErrorCodes.Unauthorized,
                "You are not authorized to perform this action."),

            KeyNotFoundException knfEx => CreateErrorResponse(
                ErrorCodes.NotFound,
                _environment.IsDevelopment()
                    ? $"The requested resource was not found: {SanitizeMessage(knfEx.Message)}"
                    : "The requested resource was not found."),

            ArgumentNullException anEx => CreateErrorResponse(
                ErrorCodes.InvalidArgument,
                $"Required parameter is missing: {SanitizeMessage(anEx.ParamName ?? "Unknown")}."),

            ArgumentException aEx => CreateErrorResponse(
                ErrorCodes.InvalidArgument,
                $"Invalid argument: {SanitizeMessage(aEx.Message)}"),

            InvalidOperationException ioEx => CreateErrorResponse(
                ErrorCodes.InvalidOperation,
                SanitizeMessage(ioEx.Message)),

            TimeoutException => CreateErrorResponse(
                ErrorCodes.ServiceTimeout,
                "The operation timed out. Please try again."),

            OperationCanceledException => CreateErrorResponse(
                ErrorCodes.OperationCancelled,
                "The operation was cancelled."),

            // Database Exceptions
            DbUpdateConcurrencyException => CreateErrorResponse(
                ErrorCodes.ConcurrencyConflict,
                "The record was modified by another user. Please refresh and try again."),

            DbUpdateException dbEx => HandleDbUpdateException(dbEx),

            // Security Exceptions
            SecurityTokenExpiredException => CreateErrorResponse(
                ErrorCodes.ExpiredToken,
                "Your session has expired. Please login again."),

            SecurityTokenValidationException => CreateErrorResponse(
                ErrorCodes.InvalidToken,
                "Token validation failed. Please login again."),

            SecurityTokenException => CreateErrorResponse(
                ErrorCodes.InvalidToken,
                "Invalid or expired token. Please login again."),

            // HTTP Exceptions (if using)
            HttpRequestException httpEx => CreateErrorResponse(
                ErrorCodes.ExternalServiceError,
                _environment.IsDevelopment()
                    ? $"External service error: {SanitizeMessage(httpEx.Message)}"
                    : "External service is temporarily unavailable. Please try again later."),

            // Default - Internal Server Error
            _ => HandleUnknownException(exception)
        };
    }

    private ApiResponse MapBaseDomainException(BaseDomainException ex)
    {
        var message = ex.UserMessage ?? SanitizeMessage(ex.Message);
        var statusCode = ErrorCodeToHttpStatusMapper.ToHttpStatusCode(ex.ErrorCode);

        // Special handling for ValidationException
        if (ex is ValidationException validationEx)
        {
            return ApiResponse.Failure(
                statusCode,
                message,
                ex.ErrorCode,
                validationEx.Errors?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToList()));
        }

        return ApiResponse.Failure(statusCode, message, ex.ErrorCode);
    }

    private static ApiResponse CreateErrorResponse(string errorCode, string message)
    {
        var statusCode = ErrorCodeToHttpStatusMapper.ToHttpStatusCode(errorCode);
        return ApiResponse.Failure(statusCode, message, errorCode);
    }

    private ApiResponse HandleDbUpdateException(DbUpdateException ex)
    {
        var innerException = ex.InnerException?.Message ?? string.Empty;

        // Unique constraint violation
        if (IsUniqueConstraintViolation(innerException))
        {
            _logger.LogWarning(ex, "[{ClassName}.{FunctionName}] - Database unique constraint violation: Error={Error}",
                nameof(GlobalExceptionHandlingMiddleware),
                nameof(HandleDbUpdateException),
                innerException);

            return CreateErrorResponse(
                ErrorCodes.DuplicateEntry,
                "A record with the same value already exists.");
        }

        // Foreign key constraint violation
        if (IsForeignKeyViolation(innerException))
        {
            _logger.LogWarning(ex, "[{ClassName}.{FunctionName}] - Database foreign key constraint violation: Error={Error}",
                nameof(GlobalExceptionHandlingMiddleware),
                nameof(HandleDbUpdateException),
                innerException);

            return CreateErrorResponse(
                ErrorCodes.ForeignKeyViolation,
                "Cannot perform this operation because it would affect related records.");
        }

        // Deadlock
        if (innerException.Contains("deadlock", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning(ex, "[{ClassName}.{FunctionName}] - Database deadlock detected: Error={Error}",
                nameof(GlobalExceptionHandlingMiddleware),
                nameof(HandleDbUpdateException),
                innerException);

            return CreateErrorResponse(
                ErrorCodes.Deadlock,
                "The operation could not be completed due to a database conflict. Please try again.");
        }

        // Connection timeout
        if (IsConnectionTimeout(innerException))
        {
            _logger.LogError(ex, "[{ClassName}.{FunctionName}] - Database connection timeout: Error={Error}",
                nameof(GlobalExceptionHandlingMiddleware),
                nameof(HandleDbUpdateException),
                innerException);

            return CreateErrorResponse(
                ErrorCodes.ConnectionTimeout,
                "Database connection timeout. Please try again.");
        }

        // Generic database error
        _logger.LogError(ex, "[{ClassName}.{FunctionName}] - Database update error: Error={Error}",
            nameof(GlobalExceptionHandlingMiddleware),
            nameof(HandleDbUpdateException),
            innerException);

        var message = _environment.IsDevelopment()
            ? $"Database error: {SanitizeMessage(ex.GetBaseException().Message)}"
            : "An error occurred while saving data. Please try again.";

        return CreateErrorResponse(ErrorCodes.DatabaseError, message);
    }

    private static bool IsUniqueConstraintViolation(string message)
    {
        return message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase) ||
               message.Contains("IX_", StringComparison.OrdinalIgnoreCase) ||
               message.Contains("PK_", StringComparison.OrdinalIgnoreCase) ||
               message.Contains("Cannot insert duplicate key", StringComparison.OrdinalIgnoreCase) ||
               message.Contains("duplicate key value", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsForeignKeyViolation(string message)
    {
        return message.Contains("FOREIGN KEY", StringComparison.OrdinalIgnoreCase) ||
               message.Contains("FK_", StringComparison.OrdinalIgnoreCase) ||
               message.Contains("DELETE statement conflicted", StringComparison.OrdinalIgnoreCase) ||
               message.Contains("UPDATE statement conflicted", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsConnectionTimeout(string message)
    {
        return message.Contains("timeout", StringComparison.OrdinalIgnoreCase) ||
               message.Contains("connection", StringComparison.OrdinalIgnoreCase) ||
               message.Contains("network", StringComparison.OrdinalIgnoreCase);
    }

    private ApiResponse HandleUnknownException(Exception exception)
    {
        _logger.LogError(
            exception,
            "[{ClassName}.{FunctionName}] - Unhandled exception: ExceptionType={ExceptionType}, Error={Error}",
            nameof(GlobalExceptionHandlingMiddleware),
            nameof(HandleUnknownException),
            exception.GetType().Name,
            exception.Message);

        var message = _environment.IsDevelopment()
            ? $"Unhandled error ({exception.GetType().Name}): {SanitizeMessage(exception.Message)}"
            : "An unexpected error occurred. Please try again later.";

        return CreateErrorResponse(ErrorCodes.InternalError, message);
    }

    private void LogException(
        Exception exception,
        HttpContext context,
        string correlationId,
        long elapsedMs,
        ApiResponse response,
        string? functionName = null)
    {
        var logLevel = DetermineLogLevel(exception);
        functionName = functionName ?? nameof(LogException);

        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["RequestPath"] = context.Request.Path.ToString(),
            ["RequestMethod"] = context.Request.Method,
            ["QueryString"] = context.Request.QueryString.ToString(),
            ["StatusCode"] = response.StatusCode,
            ["ErrorCode"] = response.ErrorCode ?? ErrorCodes.Unknown,
            ["ElapsedMs"] = elapsedMs,
            ["UserId"] = context.User?.Identity?.Name ?? "Anonymous"
        });

        // Add error details from domain exceptions
        if (exception is BaseDomainException baseException && baseException.ErrorDetails != null)
        {
            foreach (var detail in baseException.ErrorDetails)
            {
                _logger.LogDebug("ErrorDetail - {Key}: {Value}", detail.Key, detail.Value);
            }
        }

        _logger.Log(
            logLevel,
            exception,
            "[{ClassName}.{FunctionName}] - Exception handled: {ExceptionType}, ErrorCode: {ErrorCode}, Path: {RequestPath}, Method: {Method}",
            exception.GetType().Name,
            response.ErrorCode ?? "UNKNOWN",
            nameof(ExceptionHandlerExtensions),
            functionName,
            context.Request.Path,
            context.Request.Method);
    }

    private static LogLevel DetermineLogLevel(Exception exception)
    {
        return exception switch
        {
            BaseDomainException baseEx when IsInformationalError(baseEx.ErrorCode) => LogLevel.Information,
            BaseDomainException baseEx when IsWarningError(baseEx.ErrorCode) => LogLevel.Warning,
            OperationCanceledException => LogLevel.Information,
            _ => LogLevel.Error
        };
    }

    private static bool IsInformationalError(string errorCode)
    {
        return errorCode == ErrorCodes.NotFound;
    }

    private static bool IsWarningError(string errorCode)
    {
        return errorCode == ErrorCodes.ValidationError ||
               errorCode == ErrorCodes.Unauthorized ||
               errorCode == ErrorCodes.Forbidden ||
               errorCode == ErrorCodes.Conflict ||
               errorCode == ErrorCodes.RateLimitExceeded ||
               errorCode == ErrorCodes.DuplicateEntry;
    }

    /// <summary>
    /// Sanitizes error messages to prevent information leakage
    /// </summary>
    private string SanitizeMessage(string? message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return "An error occurred.";

        // In production, sanitize sensitive information
        if (!_environment.IsDevelopment())
        {
            // Remove stack traces
            if (message.Contains("at ", StringComparison.OrdinalIgnoreCase))
            {
                var firstLine = message.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                                      .FirstOrDefault() ?? message;
                return firstLine.Trim();
            }

            // Remove file paths (Windows and Unix)
            if (message.Contains(":\\", StringComparison.OrdinalIgnoreCase) ||
                message.Contains("/home/", StringComparison.OrdinalIgnoreCase) ||
                message.Contains("/var/", StringComparison.OrdinalIgnoreCase))
            {
                return "An error occurred while processing your request.";
            }

            // Remove connection strings
            if (message.Contains("Server=", StringComparison.OrdinalIgnoreCase) ||
                message.Contains("Password=", StringComparison.OrdinalIgnoreCase) ||
                message.Contains("Uid=", StringComparison.OrdinalIgnoreCase))
            {
                return "A configuration error occurred.";
            }
        }

        // Truncate overly long messages
        return message.Length > 500 ? string.Concat(message.AsSpan(0, 500), "...") : message;
    }
}