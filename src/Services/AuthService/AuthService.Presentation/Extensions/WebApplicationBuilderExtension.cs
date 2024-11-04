using System.Text;
using AuthService.BLL.Infrastructure.Mapper;
using AuthService.BLL.Infrastructure.Validators;
using AuthService.BLL.Interfaces.Services;
using AuthService.BLL.Services;
using AuthService.DAL.Infrastructure;
using AuthService.DAL.Infrastructure.Database;
using AuthService.DAL.Interfaces;
using AuthService.DAL.Interfaces.Repositories;
using AuthService.DAL.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace AuthService.Extensions;

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
            typeof(UserProfile).Assembly,
            typeof(RoleProfile).Assembly,
            typeof(AuthProfile).Assembly,
            typeof(CompanyProfile).Assembly
        );
    }

    public static void AddDatabase(this WebApplicationBuilder builder)
    {
        string? connectionString = builder.Configuration.GetConnectionString("ConnectionString");
        builder.Services.AddDbContext<AuthDbContext>(options => { options.UseNpgsql(connectionString); });
        builder.Services.AddScoped<AuthDbContext>();
    }

    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IRoleRepository, RoleRepository>();
        builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
        builder.Services.AddScoped<IAuthService, BLL.Services.AuthService>();
        builder.Services.AddScoped<IRoleService, RoleService>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<ICompanyService, CompanyService>();
        builder.Services.AddControllers();
    }

    public static void AddValidation(this WebApplicationBuilder builder)
    {
        builder
            .Services.AddFluentValidationAutoValidation()
            .AddFluentValidationClientsideAdapters();
        builder.Services.AddValidatorsFromAssemblyContaining<CreateCompanyDTOValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<GetUserByNameDTOValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<LoginDTOValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<RegisterDTOValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<RegisterUserToCompanyDTOValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<UpdateCompanyDTOValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<UpdateUserDTOValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<UserRoleDTOValidator>();

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
}
