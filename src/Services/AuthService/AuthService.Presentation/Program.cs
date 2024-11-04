using AuthService.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.AddDatabase();
builder.AddIdentity();
builder.AddMapping();
builder.AddServices();
builder.AddValidation();
builder.AddSwaggerDocumentation();
var app = builder.Build();
app.AddSwagger();
app.AddApplicationMiddleware();

app.Run();