using FitnessTracking.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessTracking.Infrastructure.Persistence.Configurations;

public class WorkoutConfiguration : IEntityTypeConfiguration<Workout>
{
    public void Configure(EntityTypeBuilder<Workout> builder)
    {
        builder.HasKey(w => w.Id);
        builder.OwnsMany(w => w.Exercises, exerciseBuilder => // сделал как owned type т к у моделей нет id 
        {
            exerciseBuilder.WithOwner().HasForeignKey("WorkoutId");
            exerciseBuilder.HasKey("WorkoutId", "Name");
        
            exerciseBuilder.OwnsMany(e => e.Sets, setBuilder =>
            {
                setBuilder.WithOwner().HasForeignKey("WorkoutId", "Name");
            });
        });

        builder.HasMany(w => w.ProgressPhotos)
            .WithOne(p => p.Workout)
            .HasForeignKey(p => p.WorkoutId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(w => w.UserId);
    }
    
}