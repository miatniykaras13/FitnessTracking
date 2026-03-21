using FitnessTracking.Infrastructure;
using Carter;
using FitnessTracking.Application;

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
        services.AddSwaggerGen();
        return services;
    }
}