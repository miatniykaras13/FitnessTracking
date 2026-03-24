using FitnessTracking.Application.Abstractions;
using FitnessTracking.Application.Abstractions.Helpers;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Infrastructure.Helpers;
using FitnessTracking.Infrastructure.Persistence;
using FitnessTracking.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FitnessTracking.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<FitnessTrackingDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("FitnessTrackingDbContext") ??
                              throw new InvalidOperationException(
                                  "Connection string 'FitnessTrackingDbContext' not found."));
        });

        services.AddScoped<IWorkoutsRepository, WorkoutsEfRepository>();
        services.AddSingleton<IMergePatchHelper, MergePatchHelper>();

        return services;
    }
}