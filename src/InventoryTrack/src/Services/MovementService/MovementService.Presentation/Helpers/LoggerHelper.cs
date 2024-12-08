namespace BookingService.Presentation.Helpers;

public class LoggerHelper<T>
{
    private readonly ILogger<T> _logger;
    
    public LoggerHelper(ILogger<T> logger)
    {
        _logger = logger;
    }
    
    public void LogStartRequest(string operationName, string paramName = "", string paramValue = "")
    {
        if (paramName=="")
        {
            _logger.LogInformation("Getting request for {Operation}",operationName);
        }
        _logger.LogInformation("Getting request for {Operation}  with {ParamName} : {ParamValue}",operationName,paramName,paramValue);
    }

    public void LogEndOfOperation(string processName, string resultOperation)
    {
        _logger.LogInformation("Successfuly {ResultOperation} after {ProcessName} process",resultOperation,processName);
    }
}