namespace NDTCore.Identity.Domain.Exceptions;

/// <summary>
/// Base exception class for all domain exceptions with enterprise features
/// </summary>
public abstract class BaseDomainException : Exception
{
    /// <summary>
    /// Error code for programmatic error handling
    /// </summary>
    public string ErrorCode { get; }

    /// <summary>
    /// Timestamp when the exception occurred
    /// </summary>
    public DateTime Timestamp { get; }

    /// <summary>
    /// Correlation ID for tracing across services
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Additional error details (key-value pairs)
    /// </summary>
    public Dictionary<string, object>? ErrorDetails { get; set; }

    /// <summary>
    /// User-friendly message (may differ from technical message)
    /// </summary>
    public string? UserMessage { get; set; }

    protected BaseDomainException(
        string errorCode,
        string message,
        Exception? innerException = null)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
        Timestamp = DateTime.UtcNow;
    }

    protected BaseDomainException(
        string errorCode,
        string message,
        string userMessage,
        Exception? innerException = null)
        : this(errorCode, message, innerException)
    {
        UserMessage = userMessage;
    }
}

