using System.Text;
using System.Text.Json.Serialization;
using FitnessTracking.Api.Constants;
using FitnessTracking.Infrastructure;
using FitnessTracking.Application;
using FitnessTracking.Shared.Constants;
using FitnessTracking.Shared.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace FitnessTracking.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddProgramDependencies(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddAuth(configuration)
            .AddApplication()
            .AddInfrastructure(configuration)
            .AddWeb();
        return services;
    }

    private static IServiceCollection AddWeb(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.Configure<JsonOptions>(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition(ApiConstants.Security.BearerScheme, new OpenApiSecurityScheme
            {
                Name = ApiConstants.Security.AuthorizationHeader,
                Type = SecuritySchemeType.Http,
                Scheme = ApiConstants.Security.BearerScheme,
                BearerFormat = ApiConstants.Security.JwtFormat,
                In = ParameterLocation.Header
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = ApiConstants.Security.BearerScheme
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
        return services;
    }

    private static IServiceCollection AddAuth(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidIssuer = configuration[AuthConstants.IssuerPath],
                    ValidAudience = configuration[AuthConstants.AudiencePath],
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration[AuthConstants.SecretPath] ??
                                               throw new MissingConfigurationException(AuthConstants.SecretPath)))
                };
            });

        services.AddAuthorizationBuilder();
        return services;
    }
    
    
}