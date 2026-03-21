using FitnessTracking.Infrastructure.Persistence;
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
        return services;
    }
}