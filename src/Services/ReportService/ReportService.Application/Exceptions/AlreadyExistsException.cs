namespace ReportService.Application.Exceptions;

public class AlreadyExistsException : ApplicationException
{
    public AlreadyExistsException(string entityName,string paramName,string paramValue):base($"The same {entityName} with {paramName} : {paramValue} already exists")
    { }
    
    public AlreadyExistsException(string message):base(message){}
}