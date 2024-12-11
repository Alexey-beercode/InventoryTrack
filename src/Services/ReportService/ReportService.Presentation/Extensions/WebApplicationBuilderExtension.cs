using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using ReportService.Application.MappingProfiles;
using ReportService.Application.Services;
using ReportService.Application.UseCases.Report.GetById;
using ReportService.Domain.Entities;
using ReportService.Domain.Interfaces.Repositories;
using ReportService.Domain.Interfaces.Services;
using ReportService.Infrastructure.Database;
using ReportService.Infrastructure.Database.Configuration;
using ReportService.Infrastructure.Messaging.Producers;
using ReportService.Infrastructure.Repositories;
using ReportService.Presentation.Validators;

namespace ReportService.Presentation.Extensions;

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
            typeof(ReportProfile).Assembly
        );
    }

    public static void AddMongoDatabase(this WebApplicationBuilder builder)
    {
        var mongoSettingsSection = builder.Configuration.GetSection("MongoDbSettings");
        builder.Services.Configure<NotificationDbSettings>(mongoSettingsSection);

        var mongoSettings = mongoSettingsSection.Get<NotificationDbSettings>();
        if (mongoSettings == null || string.IsNullOrEmpty(mongoSettings.ConnectionString) || string.IsNullOrEmpty(mongoSettings.DatabaseName))
        {
            throw new ArgumentException("MongoDB connection string or database name is not configured");
        }

        builder.Services.AddSingleton<IMongoClient>(new MongoClient(mongoSettings.ConnectionString));
        builder.Services.AddScoped<IMongoDatabase>(provider =>
        {
            var client = provider.GetRequiredService<IMongoClient>();
            return client.GetDatabase(mongoSettings.DatabaseName);
        });

        builder.Services.AddScoped<IMongoCollection<Report>>(provider =>
        {
            var database = provider.GetRequiredService<IMongoDatabase>();
            return database.GetCollection<Report>("reports");
        });

        builder.Services.AddScoped<ReportDbContext>();
    }

    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IReportRepository, ReportRepository>();
        builder.Services.AddScoped<IExcelExportService, ExcelExportService>();
        builder.Services.AddControllers();
    }

    public static void AddValidation(this WebApplicationBuilder builder)
    {
        builder
            .Services.AddFluentValidationAutoValidation()
            .AddFluentValidationClientsideAdapters();
        builder.Services.AddValidatorsFromAssemblyContaining<GetPaginatedReportsQueryValidator>();
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
    
    public static void AddMediatr(this WebApplicationBuilder builder)
    {
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
            typeof(GetReportByIdQuery).Assembly,
            typeof(GetReportByIdQueryHandler).Assembly
        ));
    }
    
    public static void AddMassTransitWithRabbitMq(this WebApplicationBuilder builder)
    {
        var rabbitMqSettings = builder.Configuration.GetSection("RabbitMQ");

        builder.Services.AddMassTransit(x =>
        {
            // Регистрация всех IRequestClient
            x.AddRequestClient<GetReportDataMessage>();
            x.AddRequestClient<GetUserMessage>();
            x.AddRequestClient<GetItemMessage>();
            x.AddRequestClient<GetWarehouseMessage>();

            // Конфигурация RabbitMQ
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitMqSettings["Hostname"], "/", h =>
                {
                    h.Username(rabbitMqSettings["Username"]);
                    h.Password(rabbitMqSettings["Password"]);
                });

                // Конфигурация очередей и endpoint'ов
                cfg.ConfigureEndpoints(context);
            });
        });

        // Регистрация ReportProducer
        builder.Services.AddScoped<ReportProducer>();

        // Включение HostedService для MassTransit
        builder.Services.AddMassTransitHostedService();
    }

}
