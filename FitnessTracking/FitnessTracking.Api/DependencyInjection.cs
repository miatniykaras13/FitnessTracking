using FitnessTracking.Infrastructure;

namespace FitnessTracking.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddProgramDependencies(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddInfrastructure(configuration)
            .AddWeb(configuration);
        return services;
    }

    private static IServiceCollection AddWeb(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        return services;
    }
}