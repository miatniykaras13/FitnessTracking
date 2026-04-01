using FitnessTracking.Domain.Models;

namespace FitnessTracking.Application.Abstractions.Repositories;

public interface IWorkoutPhotosRepository : IRepository<WorkoutPhoto, Guid>
{
}