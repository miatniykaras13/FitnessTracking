using CSharpFunctionalExtensions;
using FitnessTracking.Application.Filters;
using FitnessTracking.Application.Paging;
using FitnessTracking.Application.Sorting;
using FitnessTracking.Shared.Contracts.Requests;
using FitnessTracking.Shared.Contracts.Responses;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Abstractions;

public interface IWorkoutsService
{
    Task<Result<WorkoutResponse, Error>> GetByIdAsync(GetWorkoutByIdRequest request, CancellationToken ct);

    Task<Result<CreateWorkoutResponse, Error>> AddAsync(CreateWorkoutRequest request, CancellationToken ct);

    Task<Result<WorkoutResponse, Error>> UpdateAsync(UpdateWorkoutRequest request, CancellationToken ct);

    Task<Result<WorkoutResponse, Error>> PatchAsync(PatchWorkoutRequest request, CancellationToken ct);

    Task<UnitResult<Error>> DeleteAsync(DeleteWorkoutRequest request, CancellationToken ct);

    Task<Result<WorkoutListResponse, Error>> GetByUserIdAsync(GetWorkoutsByUserIdRequest request, WorkoutFilter filter,
        SortParameters sortParameters,
        PageParameters pageParameters,
        CancellationToken ct);

    Task<Result<WorkoutExercisesResponse, Error>> GetExercisesByWorkoutIdAsync(
        GetExercisesByWorkoutIdRequest request,
        CancellationToken ct);

    Task<Result<ExerciseResponse, Error>> AddExerciseAsync(AddExerciseRequest request, CancellationToken ct);

    Task<Result<ExerciseResponse, Error>> UpdateExerciseAsync(UpdateExerciseRequest request, CancellationToken ct);

    Task<Result<ExerciseResponse, Error>> PatchExerciseAsync(PatchExerciseRequest request, CancellationToken ct);

    Task<Result<WorkoutExercisesResponse, Error>> UpdateExercisesAsync(UpdateExercisesRequest request, CancellationToken ct);

    Task<UnitResult<Error>> DeleteExerciseAsync(DeleteExerciseRequest request, CancellationToken ct);

    Task<Result<SetResponse, Error>> AddSetAsync(AddSetRequest request, CancellationToken ct);

    Task<Result<SetResponse, Error>> UpdateSetAsync(UpdateSetRequest request, CancellationToken ct);

    Task<Result<SetResponse, Error>> PatchSetAsync(PatchSetRequest request, CancellationToken ct);

    Task<Result<SetListResponse, Error>> UpdateSetsAsync(UpdateSetsRequest request, CancellationToken ct);

    Task<UnitResult<Error>> DeleteSetAsync(DeleteSetRequest request, CancellationToken ct);

    Task<Result<AddPhotosToWorkoutResponse, Error>> AddPhotosToWorkoutAsync(
        AddPhotosToWorkoutRequest request,
        CancellationToken ct);
}

