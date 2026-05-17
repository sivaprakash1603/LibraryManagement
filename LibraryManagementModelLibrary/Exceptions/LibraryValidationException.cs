namespace LibraryManagementModelLibrary.Exceptions;

public class LibraryValidationException : LibraryManagementException
{
    public LibraryValidationException(string message)
        : base(message, "VALIDATION_FAILED")
    {
    }
}
