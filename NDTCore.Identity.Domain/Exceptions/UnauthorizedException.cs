using NDTCore.Identity.Domain.Constants;

namespace NDTCore.Identity.Domain.Exceptions;

/// <summary>
/// Exception thrown when authentication is required or fails
/// </summary>
public class UnauthorizedException : BaseDomainException
{
    public UnauthorizedException(string message = "Unauthorized access.")
        : base(ErrorCodes.Unauthorized, message, "You are not authorized to perform this action.") { }

    public UnauthorizedException(string message, string userMessage)
        : base(ErrorCodes.Unauthorized, message, userMessage) { }
}
