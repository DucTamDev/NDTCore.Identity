namespace NDTCore.Identity.Application.Common.Exceptions;

/// <summary>
/// Business rule violation exception
/// </summary>
public class BusinessRuleViolationException : ApplicationException
{
    public string? RuleName { get; }

    public BusinessRuleViolationException(string message) : base(message)
    {
    }

    public BusinessRuleViolationException(string message, string ruleName) : base(message)
    {
        RuleName = ruleName;
    }

    public BusinessRuleViolationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

