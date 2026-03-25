using FitnessTracking.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracking.Infrastructure.Persistence;

public class FitnessTrackingDbContext(DbContextOptions<FitnessTrackingDbContext> options) : DbContext(options)
{
    public DbSet<Workout> Workouts { get; set; }
    
    public DbSet<WorkoutPhoto> WorkoutPhotos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FitnessTrackingDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}