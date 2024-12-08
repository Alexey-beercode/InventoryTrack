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
using ReportService.Infrastructure.Repositories;

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

    public static void AddMassTransit(this WebApplicationBuilder builder)
    {
        builder.Services.AddMassTransit(config =>
        {
            config.UsingRabbitMq((context, cfg) =>
            {
                var rabbitSettings = builder.Configuration.GetSection("RabbitMQ");
                var host = rabbitSettings["Host"];
                var username = rabbitSettings["Username"];
                var password = rabbitSettings["Password"];

                cfg.Host(host, h =>
                {
                    h.Username(username);
                    h.Password(password);
                });

                // Добавляем запрос клиента
                cfg.ConfigureEndpoints(context);
            });
        });
    }

    /*public static void AddValidation(this WebApplicationBuilder builder)
    {
        builder
            .Services.AddFluentValidationAutoValidation()
            .AddFluentValidationClientsideAdapters();
    }*/

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
}
