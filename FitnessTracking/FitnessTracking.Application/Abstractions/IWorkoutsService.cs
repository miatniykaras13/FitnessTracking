using CSharpFunctionalExtensions;
using FitnessTracking.Shared.Contracts.Requests;
using FitnessTracking.Shared.Contracts.Responses;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Abstractions;

public interface IWorkoutsService
{
    Task<Result<WorkoutResponse, Error>> GetByIdAsync(GetWorkoutByIdRequest request, CancellationToken ct);

    Task<Result<CreateWorkoutResponse, Error>> AddAsync(CreateWorkoutRequest request, CancellationToken ct);

    Task<Result<UpdateWorkoutResponse, Error>> UpdateAsync(UpdateWorkoutRequest request, CancellationToken ct);

    Task<UnitResult<Error>> DeleteAsync(DeleteWorkoutRequest request, CancellationToken ct);

    Task<Result<WorkoutListResponse, Error>> GetByUserIdAsync(GetWorkoutsByUserIdRequest request, CancellationToken ct);

    Task<Result<WorkoutExercisesResponse, Error>> GetExercisesByWorkoutIdAsync(
        GetExercisesByWorkoutIdRequest request,
        CancellationToken ct);

    Task<Result<AddPhotosToWorkoutResponse, Error>> AddPhotosToWorkoutAsync(
        AddPhotosToWorkoutRequest request,
        CancellationToken ct);
}

