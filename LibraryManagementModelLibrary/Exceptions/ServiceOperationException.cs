namespace LibraryManagementModelLibrary.Exceptions;

public class ServiceOperationException : LibraryManagementException
{
    public ServiceOperationException(string operation, Exception innerException)
        : base($"Service operation failed: {operation}", "SERVICE_OPERATION_FAILED", innerException)
    {
    }
}
