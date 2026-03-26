using FitnessTracking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracking.Api;

public static class ApiExtensions
{
    public static async Task<WebApplication> UseMigrating(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<FitnessTrackingDbContext>();
            await dbContext.Database.EnsureCreatedAsync();
        }
        return app;
    }
}