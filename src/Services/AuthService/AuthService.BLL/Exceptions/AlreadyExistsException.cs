namespace EventMaster.BLL.Exceptions;

public class AlreadyExistsException : ApplicationException
{
    public AlreadyExistsException(string entityName):base($"The same{entityName} already exists")
    {
    }
}