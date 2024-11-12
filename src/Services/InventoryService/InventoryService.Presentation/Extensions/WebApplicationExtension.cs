using InventoryService.Middleware;

namespace InventoryService.Extensions;

public static class WebApplicationExtension
{
    public static void AddSwagger(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
    }
    public static void AddApplicationMiddleware(this WebApplication app)
    {
        app.UseHttpsRedirection(); 
        app.UseRouting(); 
        
        app.UseCors(builder =>
        {
            builder.AllowAnyOrigin() 
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