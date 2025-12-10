using NDTCore.Identity.Domain.Constants;

namespace NDTCore.Identity.Domain.Exceptions;

/// <summary>
/// General domain exception for business rule violations
/// </summary>
public class DomainException : BaseDomainException
{
    public DomainException(string message)
        : base(ErrorCodes.BusinessRuleViolation, message) { }

    public DomainException(string message, Exception innerException)
        : base(ErrorCodes.BusinessRuleViolation, message, innerException) { }

    public DomainException(string message, string userMessage)
        : base(ErrorCodes.BusinessRuleViolation, message, userMessage) { }

    public DomainException(string message, string userMessage, Exception innerException)
        : base(ErrorCodes.BusinessRuleViolation, message, userMessage, innerException) { }
}
