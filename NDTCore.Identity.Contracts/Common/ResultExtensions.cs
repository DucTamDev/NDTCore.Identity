
namespace NDTCore.Identity.Contracts.Common;

/// <summary>
/// Extension methods for converting Result pattern to ApiResponse
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Converts Result<T> to ApiResponse<T>
    /// </summary>
    /// <typeparam name="T">Type of the result value</typeparam>
    /// <param name="result">The result to convert</param>
    /// <param name="successMessage">Success message (default: "Success")</param>
    /// <param name="successStatusCode">Success HTTP status code (default: 200)</param>
    /// <param name="failureStatusCode">Failure HTTP status code (default: 400)</param>
    /// <returns>ApiResponse<T></returns>
    public static ApiResponse<T> ToApiResponse<T>(
        this Result<T> result,
        string? successMessage = null,
        int successStatusCode = 200,
        int failureStatusCode = 400)
    {
        if (result.IsSuccess && result.Value != null)
        {
            return ApiResponse<T>.SuccessResponse(
                result.Value,
                successMessage ?? "Success",
                successStatusCode);
        }

        // Map error to appropriate status code
        var statusCode = MapErrorToStatusCode(result.Error ?? "Operation failed", failureStatusCode);

        return ApiResponse<T>.FailureResponse(
            result.Error ?? "Operation failed",
            statusCode,
            result.Errors);
    }

    /// <summary>
    /// Converts Result to ApiResponse
    /// </summary>
    /// <param name="result">The result to convert</param>
    /// <param name="successMessage">Success message (default: "Success")</param>
    /// <param name="successStatusCode">Success HTTP status code (default: 200)</param>
    /// <param name="failureStatusCode">Failure HTTP status code (default: 400)</param>
    /// <returns>ApiResponse</returns>
    public static ApiResponse ToApiResponse(
        this Result result,
        string? successMessage = null,
        int successStatusCode = 200,
        int failureStatusCode = 400)
    {
        if (result.IsSuccess)
        {
            return ApiResponse.SuccessResponse(
                successMessage ?? "Success",
                successStatusCode);
        }

        // Map error to appropriate status code
        var statusCode = MapErrorToStatusCode(result.Error ?? "Operation failed", failureStatusCode);

        return ApiResponse.FailureResponse(
            result.Error ?? "Operation failed",
            statusCode,
            result.Errors);
    }

    /// <summary>
    /// Maps error message to appropriate HTTP status code
    /// </summary>
    private static int MapErrorToStatusCode(string errorMessage, int defaultStatusCode)
    {
        if (string.IsNullOrEmpty(errorMessage))
            return defaultStatusCode;

        var lowerError = errorMessage.ToLowerInvariant();

        // Not found errors
        if (lowerError.Contains("not found") || lowerError.Contains("does not exist"))
            return 404;

        // Authentication errors
        if (lowerError.Contains("invalid") && 
            (lowerError.Contains("password") || lowerError.Contains("token") || lowerError.Contains("email") || lowerError.Contains("credential")))
            return 401;

        // Authorization errors
        if (lowerError.Contains("forbidden") || lowerError.Contains("unauthorized") || lowerError.Contains("access denied") || lowerError.Contains("permission"))
            return 403;

        // Validation errors
        if (lowerError.Contains("validation") || lowerError.Contains("invalid") || lowerError.Contains("required"))
            return 400;

        // Conflict errors
        if (lowerError.Contains("already exists") || lowerError.Contains("duplicate") || lowerError.Contains("conflict"))
            return 409;

        // Default
        return defaultStatusCode;
    }
}

