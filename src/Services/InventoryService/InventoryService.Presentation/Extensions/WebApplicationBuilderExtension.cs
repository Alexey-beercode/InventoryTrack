using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using InventoryService.Application.Facades;
using InventoryService.Application.Interfaces.Facades;
using InventoryService.Application.Interfaces.Services;
using InventoryService.Application.MapperProfiles;
using InventoryService.Application.Services;
using InventoryService.Domain.Interfaces.Repositories;
using InventoryService.Domain.Interfaces.UnitOfWork;
using InventoryService.Infrastructure;
using InventoryService.Infrastructure.Config.Database;
using InventoryService.Infrastructure.Messaging.Consumers;
using InventoryService.Infrastructure.Repositories;
using InventoryService.Validators;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace InventoryService.Extensions;

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
            typeof(InventoryItemProfile).Assembly,
            typeof(SupplierProfile).Assembly,
            typeof(WarehouseProfile).Assembly
        );
    }

    public static void AddDatabase(this WebApplicationBuilder builder)
    {
        string? connectionString = builder.Configuration.GetConnectionString("ConnectionString");
        builder.Services.AddDbContext<InventoryDbContext>(options => { options.UseNpgsql(connectionString); });
        builder.Services.AddScoped<InventoryDbContext>();
    }

    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
        builder.Services.AddScoped<IInventoryItemRepository, InventoryItemRepository>();
        builder.Services.AddScoped<IInventoriesItemsWarehousesRepository, InventoriesItemsWarehousesRepository>();
        builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
        builder.Services.AddScoped<IWarehouseRepository, WarehouseRepository>();
        builder.Services.AddScoped<IDocumentService, DocumentService>();
        builder.Services.AddScoped<IInventoryItemService, InventoryItemService>();
        builder.Services.AddScoped<ISupplierService, SupplierService>();
        builder.Services.AddScoped<IWarehouseService, WarehouseService>();
        builder.Services.AddScoped<IInventoryItemFacade, InventoryItemFacade>();
        builder.Services.AddControllers();
    }

    public static void AddValidation(this WebApplicationBuilder builder)
    {
        builder
            .Services.AddFluentValidationAutoValidation()
            .AddFluentValidationClientsideAdapters();
        builder.Services.AddValidatorsFromAssemblyContaining<CreateInventoryItemDtoValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<CreateSupplierDtoValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<CreateWarehouseDtoValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<FilterInventoryItemDtoValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<UpdateInventoryItemDtoValidator>();
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
            x.AddConsumer<MovementsRequestConsumer>();
            x.AddConsumer<WriteOffsRequestConsumer>();

            // Регистрация IRequestClient для запросов
            x.AddRequestClient<GetReportDataMessage>();
            x.AddRequestClient<WriteOffInventoryMessage>();
            x.AddRequestClient<MoveInventoryMessage>();

            // Конфигурация RabbitMQ
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitMqSettings["Hostname"], "/", h =>
                {
                    h.Username(rabbitMqSettings["Username"]);
                    h.Password(rabbitMqSettings["Password"]);
                });

                cfg.ReceiveEndpoint("move-inventory-queue", e =>
                {
                    e.ConfigureConsumer<MovementsRequestConsumer>(context);
                });

                cfg.ReceiveEndpoint("write-off-queue", e =>
                {
                    e.ConfigureConsumer<WriteOffsRequestConsumer>(context);
                });
            });
        });

        // Включение HostedService для MassTransit
        builder.Services.AddMassTransitHostedService();
    }

}
