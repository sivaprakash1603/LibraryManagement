namespace LibraryManagementModelLibrary.Exceptions;

public class LibraryManagementException : Exception
{
    public string? ErrorCode { get; }

    public LibraryManagementException(string message)
        : base(message)
    {
    }

    public LibraryManagementException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public LibraryManagementException(string message, string errorCode, Exception? innerException = null)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}
