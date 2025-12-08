using NDTCore.Identity.Domain.Constants;

namespace NDTCore.Identity.Domain.Exceptions;

/// <summary>
/// Exception thrown when access is forbidden (authorization fails)
/// </summary>
public class ForbiddenException : BaseDomainException
{
    public ForbiddenException(string message = "Access forbidden.")
        : base(ErrorCodes.Forbidden, message, "You do not have permission to perform this action.") { }

    public ForbiddenException(string message, string userMessage)
        : base(ErrorCodes.Forbidden, message, userMessage) { }
}
