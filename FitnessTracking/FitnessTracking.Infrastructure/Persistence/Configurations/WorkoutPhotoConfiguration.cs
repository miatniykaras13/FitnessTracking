using FitnessTracking.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessTracking.Infrastructure.Persistence.Configurations;

public class WorkoutPhotoConfiguration : IEntityTypeConfiguration<WorkoutPhoto>
{
    public void Configure(EntityTypeBuilder<WorkoutPhoto> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Path).IsRequired();

        builder.HasOne(p => p.Workout)
            .WithMany(w => w.ProgressPhotos)
            .HasForeignKey(p => p.WorkoutId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
