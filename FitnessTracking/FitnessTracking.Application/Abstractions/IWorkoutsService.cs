using FitnessTracking.Shared.Contracts.Requests;
using FitnessTracking.Shared.Contracts.Responses;

namespace FitnessTracking.Application.Abstractions;

public interface IWorkoutsService
{
    Task<WorkoutResponse> GetByIdAsync(GetWorkoutByIdRequest request, CancellationToken ct);

    Task<CreateWorkoutResponse> AddAsync(CreateWorkoutRequest request, CancellationToken ct);

    Task<UpdateWorkoutResponse> UpdateAsync(UpdateWorkoutRequest request, CancellationToken ct);

    Task DeleteAsync(DeleteWorkoutRequest request, CancellationToken ct);

    Task<WorkoutListResponse> GetByUserIdAsync(GetWorkoutsByUserIdRequest request, CancellationToken ct);

    Task<WorkoutExercisesResponse> GetExercisesByWorkoutIdAsync(GetWorkoutByIdRequest request, CancellationToken ct);

    Task AddPhotosToWorkoutAsync(Guid workoutId, CancellationToken ct);
}

