using WriteOffService.Presentation.Middleware;

namespace WriteOffService.Presentation.Extensions;

public static class WebApplicationExtension
{
    public static void AddSwagger(this WebApplication app)
    {
            app.UseSwagger();
            app.UseSwaggerUI();
    }
    public static void AddApplicationMiddleware(this WebApplication app)
    {
        app.UseHttpsRedirection(); 
        app.UseRouting(); 
        
        app.UseCors(builder =>
        {
            builder
                .WithOrigins("http://localhost:4200")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        }); 
        
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        
        app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}