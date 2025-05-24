using Microsoft.EntityFrameworkCore;
using WriteOffService.Infrastructure.Config.Database;
using WriteOffService.Presentation.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.AddDatabase();
builder.AddIdentity();
builder.AddMapping();
builder.AddServices();
builder.AddValidation();
builder.AddSwaggerDocumentation();
builder.AddMassTransitWithRabbitMq();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<WriteOffDbContext>();
        db.Database.Migrate(); 
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database");
    }
}

app.AddSwagger();
app.AddApplicationMiddleware();
app.Run();