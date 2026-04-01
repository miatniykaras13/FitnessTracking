using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Domain.Models;
namespace FitnessTracking.Infrastructure.Persistence.Repositories;

public class WorkoutPhotosEfRepository(FitnessTrackingDbContext dbContext)
    : Repository<WorkoutPhoto, Guid>(dbContext, dbContext.WorkoutPhotos), IWorkoutPhotosRepository;
