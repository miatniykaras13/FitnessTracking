using FitnessTracking.Domain.Models;
using CSharpFunctionalExtensions;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Abstractions.Repositories;

public interface IWorkoutPhotosRepository : IRepository<WorkoutPhoto, Guid>
{
}