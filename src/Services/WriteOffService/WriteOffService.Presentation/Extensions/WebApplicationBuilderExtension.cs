using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WriteOffService.Application.Clients;
using WriteOffService.Application.Interfaces.Clients;
using WriteOffService.Application.Interfaces.Services;
using WriteOffService.Application.MapperProfiles;
using WriteOffService.Application.Messaging.Producers;
using WriteOffService.Application.Services;
using WriteOffService.Domain.Interfaces.Repositories;
using WriteOffService.Domain.Interfaces.UnitOfWork;
using WriteOffService.Infrastructure;
using WriteOffService.Infrastructure.Config.Database;
using WriteOffService.Infrastructure.Repositories;
using WriteOffService.Presentation.Validators;

namespace WriteOffService.Presentation.Extensions;

public static class WebApplicationBuilderExtension
{
    public static void AddSwaggerDocumentation(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Description = @"Enter JWT Token please.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                }
            );
            options.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                        },
                        new List<string>()
                    }
                }
            );
        });
    }

    public static void AddMapping(this WebApplicationBuilder builder)
    {
        builder.Services.AddAutoMapper(
            typeof(DocumentProfile).Assembly,
            typeof(RequestStatusProfile).Assembly,
            typeof(WriteOffReasonProfile).Assembly,
            typeof(WriteOffRequestProfile).Assembly
        );
    }

    public static void AddDatabase(this WebApplicationBuilder builder)
    {
        string? connectionString = builder.Configuration.GetConnectionString("ConnectionString");
        builder.Services.AddDbContext<WriteOffDbContext>(options => { options.UseNpgsql(connectionString); });
        builder.Services.AddScoped<WriteOffDbContext>();
    }

    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
        builder.Services.AddScoped<IWriteOffActRepository, WriteOffActRepository>();
        builder.Services.AddScoped<IWriteOffReasonRepository, WriteOffReasonRepository>();
        builder.Services.AddScoped<IWriteOffRequestRepository, WriteOffRequestRepository>();
        builder.Services.AddScoped<IDocumentService, DocumentService>();
        builder.Services.AddScoped<IWriteOffReasonService, WriteOffReasonService>();
        builder.Services.AddScoped<IWriteOffRequestService, WriteOffRequestService>();
        builder.Services.AddHttpClient<IInventoryHttpClient, InventoryHttpClient>();
        builder.Services.AddScoped<IInventoryHttpClient, InventoryHttpClient>();
        builder.Services.AddControllers();
    }

    public static void AddValidation(this WebApplicationBuilder builder)
    {
        builder
            .Services.AddFluentValidationAutoValidation()
            .AddFluentValidationClientsideAdapters();
        builder.Services.AddValidatorsFromAssemblyContaining<CreateWriteOffRequestDtoValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<UpdateWriteOffRequestDtoValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<WriteOffRequestFilterDtoValidator>();
    }

    public static void AddIdentity(this WebApplicationBuilder builder)
    {
        var jwtSettings = builder.Configuration.GetSection("Jwt");

        var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"]);
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                LogValidationExceptions = true
            };
        });

        builder.Services.AddAuthorization(options =>
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();

            options.AddPolicy("Accountant", policy => { policy.RequireRole("Accountant"); });
            options.AddPolicy("Warehouse Manager", policy => { policy.RequireRole("Warehouse Manager"); });
            options.AddPolicy("Department Head", policy => { policy.RequireRole("Department Head"); });
                options.AddPolicy("WarehouseOrDepartmentHead", policy =>
                    policy.RequireRole("Warehouse Manager", "Department Head"));

            
        });
    }
    
    public static void AddMassTransitWithRabbitMq(this WebApplicationBuilder builder)
    {
        var rabbitMqSettings = builder.Configuration.GetSection("RabbitMQ");

        builder.Services.AddMassTransit(x =>
        {

            // Регистрация Producer через IPublishEndpoint
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitMqSettings["Hostname"], "/", h =>
                {
                    h.Username(rabbitMqSettings["Username"]);
                    h.Password(rabbitMqSettings["Password"]);
                });
            });
        });

        // Регистрация IPublishEndpoint для Producer
        builder.Services.AddScoped<WriteOffsRequestProducer>();

        // Включение MassTransit как HostedService
        builder.Services.AddMassTransitHostedService();
    }

}
