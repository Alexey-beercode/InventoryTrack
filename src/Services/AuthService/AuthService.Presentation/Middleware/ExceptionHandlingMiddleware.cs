using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Authentication;
using AuthService.BLL.Exceptions;
using EventMaster.BLL.Exceptions;
using Newtonsoft.Json;

namespace AuthService.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }
    
    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var response = context.Response;

        switch (exception)
        {
            case InvalidCredentialException:
                response.StatusCode = (int)HttpStatusCode.NotAcceptable;
                break;
            case EntityNotFoundException:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                break;
            case FormatException:
            case ValidationException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;
            case UnauthorizedAccessException:
            case AuthenticationException:
            case AuthorizationException:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                break;
            case BadRequestException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;
            case AlreadyExistsException:
                response.StatusCode = (int)HttpStatusCode.Conflict;
                break;
            default:
                // Для всех остальных исключений устанавливаем статус 500 (Internal Server Error)
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
        }
        
        
        var result = JsonConvert.SerializeObject(new 
        {
            error = exception.Message,
            details = exception.StackTrace
        });
        _logger.LogError(result);
        
        return context.Response.WriteAsync(result);
    }
}