using NDTCore.Identity.Domain.Enums;

namespace NDTCore.Identity.Contracts.Common;

/// <summary>
/// Represents the result of a domain/repository operation
/// Used internally within application layers (not exposed to API)
/// </summary>
public class OperationResult<TValue>
{
    public bool IsSuccess { get; protected init; }
    public bool IsFailure => !IsSuccess;
    public TValue? Data { get; protected init; }
    public OperationError? Error { get; protected init; }

    // Private constructor - force use of factory methods
    protected OperationResult() { }

    /// <summary>
    /// Creates a successful result
    /// </summary>
    public static OperationResult<TValue> Success(TValue value)
    {
        return new OperationResult<TValue>
        {
            IsSuccess = true,
            Data = value,
            Error = null
        };
    }

    /// <summary>
    /// Creates a failed result with error details
    /// </summary>
    public static OperationResult<TValue> Failure(
        string message,
        OperationErrorType errorType = OperationErrorType.BusinessRuleViolation,
        Dictionary<string, string[]>? validationErrors = null)
    {
        return new OperationResult<TValue>
        {
            IsSuccess = false,
            Data = default,
            Error = new OperationError
            {
                Message = message,
                ErrorType = errorType,
                ValidationErrors = validationErrors ?? new Dictionary<string, string[]>()
            }
        };
    }

    /// <summary>
    /// Creates a not found result
    /// </summary>
    public static OperationResult<TValue> NotFound(string entityName, object key)
    {
        return Failure(
            $"{entityName} with key '{key}' was not found",
            OperationErrorType.NotFound
        );
    }

    /// <summary>
    /// Creates a validation failure result
    /// </summary>
    public static OperationResult<TValue> ValidationFailure(
        Dictionary<string, string[]> validationErrors)
    {
        return Failure(
            "One or more validation errors occurred",
            OperationErrorType.ValidationError,
            validationErrors
        );
    }
}

/// <summary>
/// Non-generic operation result for operations that don't return data
/// </summary>
public class OperationResult : OperationResult<object>
{
    public static OperationResult Success()
    {
        return new OperationResult
        {
            IsSuccess = true,
            Data = null,
            Error = null
        };
    }

    public static new OperationResult Failure(
        string message,
        OperationErrorType errorType = OperationErrorType.BusinessRuleViolation,
        Dictionary<string, string[]>? validationErrors = null)
    {
        return new OperationResult
        {
            IsSuccess = false,
            Data = null,
            Error = new OperationError
            {
                Message = message,
                ErrorType = errorType,
                ValidationErrors = validationErrors ?? new Dictionary<string, string[]>()
            }
        };
    }
}