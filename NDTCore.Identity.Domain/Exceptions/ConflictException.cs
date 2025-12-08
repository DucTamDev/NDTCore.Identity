using NDTCore.Identity.Domain.Constants;

namespace NDTCore.Identity.Domain.Exceptions;

/// <summary>
/// Exception thrown when a conflict occurs (e.g., duplicate entry, concurrency conflict)
/// </summary>
public class ConflictException : BaseDomainException
{
    public ConflictException(string message)
        : base(ErrorCodes.Conflict, message) { }

    public ConflictException(string message, Exception innerException)
        : base(ErrorCodes.Conflict, message, innerException) { }

    public ConflictException(string message, string userMessage)
        : base(ErrorCodes.Conflict, message, userMessage) { }

    public ConflictException(string message, string userMessage, Exception innerException)
        : base(ErrorCodes.Conflict, message, userMessage, innerException) { }
}

