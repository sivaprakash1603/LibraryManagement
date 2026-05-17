namespace LibraryManagementModelLibrary.Exceptions;

public class BusinessRuleViolationException : LibraryManagementException
{
    public BusinessRuleViolationException(string message)
        : base(message, "BUSINESS_RULE_VIOLATION")
    {
    }
}
