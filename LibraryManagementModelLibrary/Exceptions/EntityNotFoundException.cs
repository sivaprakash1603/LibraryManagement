namespace LibraryManagementModelLibrary.Exceptions;

public class EntityNotFoundException : LibraryManagementException
{
    public EntityNotFoundException(string entityName, string key)
        : base($"{entityName} was not found. Key: {key}", "ENTITY_NOT_FOUND")
    {
    }
}
