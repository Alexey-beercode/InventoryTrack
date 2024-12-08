namespace EventMaster.BLL.Exceptions;

public class EntityNotFoundException:Exception
{
    public EntityNotFoundException(string message) : base(message)
    {
    }

    public EntityNotFoundException(string entityName, Guid id): base($"Can't find entity of type {entityName} with ID {id}.") { }
}