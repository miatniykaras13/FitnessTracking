using System.Text.Json.Serialization;
using FitnessTracking.Infrastructure;
using Carter;
using FitnessTracking.Application;
using Microsoft.AspNetCore.Http.Json;

namespace FitnessTracking.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddProgramDependencies(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddApplication()
            .AddInfrastructure(configuration)
            .AddWeb(configuration);
        return services;
    }

    private static IServiceCollection AddWeb(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddCarter();
        services.AddEndpointsApiExplorer();
        services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
        services.AddSwaggerGen();
        return services;
    }
}