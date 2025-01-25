using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MovementService.Application.Interfaces.Services;
using MovementService.Application.MapperProfiles;
using MovementService.Application.Services;
using MovementService.Domain.Interfaces.Repositories;
using MovementService.Domain.Interfaces.UnitOfWork;
using MovementService.Infrastructure;
using MovementService.Infrastructure.Config.Database;
using MovementService.Infrastructure.Messaging.Producers;
using MovementService.Infrastructure.Repositories;
using MovementService.Presentation.Validators;

namespace MovementService.Presentation.Extensions;

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
            typeof(MovementRequestProfile).Assembly
        );
    }

    public static void AddDatabase(this WebApplicationBuilder builder)
    {
        string? connectionString = builder.Configuration.GetConnectionString("ConnectionString");
        builder.Services.AddDbContext<MovementDbContext>(options => { options.UseNpgsql(connectionString); });
        builder.Services.AddScoped<MovementDbContext>();
    }

    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped<IMovementRequestRepository, MovementRequestRepository>();
        builder.Services.AddScoped<IMovementRequestService, MovementRequestService>();
        builder.Services.AddControllers();
    }

    public static void AddValidation(this WebApplicationBuilder builder)
    {
        builder
            .Services.AddFluentValidationAutoValidation()
            .AddFluentValidationClientsideAdapters();
        builder.Services.AddValidatorsFromAssemblyContaining<CreateMovementRequestValidator>();
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
        builder.Services.AddScoped<MovementRequestProducer>();

        // Включение MassTransit как HostedService
        builder.Services.AddMassTransitHostedService();
    }

}
