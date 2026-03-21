using FitnessTracking.Application.Abstractions;
using FitnessTracking.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FitnessTracking.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IWorkoutsService, WorkoutsService>();
        return services;
    }
}