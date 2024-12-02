using ReportService.Presentation.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.AddIdentity();
builder.AddMapping();
builder.AddMediatr();
builder.AddServices();
builder.AddValidation();
builder.AddMongoDatabase();
builder.AddSwaggerDocumentation();
var app = builder.Build();

app.AddSwagger();
app.AddApplicationMiddleware();

app.Run();