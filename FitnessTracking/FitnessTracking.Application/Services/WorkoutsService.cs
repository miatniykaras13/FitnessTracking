using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions;
using FitnessTracking.Application.Abstractions.Helpers;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Filters;
using FitnessTracking.Application.Paging;
using FitnessTracking.Application.Sorting;
using FitnessTracking.Shared.Contracts.Dtos;
using FitnessTracking.Domain.Enums;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Contracts.Requests;
using FitnessTracking.Shared.Contracts.Responses;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Services;

public class WorkoutsService(
    IWorkoutsRepository repository,
    IMergePatchHelper mergePatchHelper) : IWorkoutsService
{
    public async Task<Result<WorkoutResponse, Error>> GetByIdAsync(GetWorkoutByIdRequest request, CancellationToken ct)
    {
        var workoutResult = await repository.GetByIdAsync(request.WorkoutId, ct);

        if (workoutResult.IsFailure)
        {
            return Result.Failure<WorkoutResponse, Error>(workoutResult.Error);
        }

        return Result.Success<WorkoutResponse, Error>(MapToResponse(workoutResult.Value));
    }

    public async Task<Result<CreateWorkoutResponse, Error>> AddAsync(CreateWorkoutRequest request, CancellationToken ct)
    {
        if (request.UserId == Guid.Empty)
        {
            return Result.Failure<CreateWorkoutResponse, Error>(WorkoutErrors.UserIdRequired());
        }

        var workoutValidationError = ValidateWorkoutFields(
            request.WorkoutDto.Title,
            request.WorkoutDto.Type,
            request.WorkoutDto.Duration,
            request.WorkoutDto.CaloriesBurned,
            request.WorkoutDto.WorkoutDate);
        if (workoutValidationError is not null)
        {
            return Result.Failure<CreateWorkoutResponse, Error>(workoutValidationError);
        }

        if (!Enum.TryParse<WorkoutType>(request.WorkoutDto.Type, true, out var workoutType))
        {
            return Result.Failure<CreateWorkoutResponse, Error>(WorkoutErrors.InvalidWorkoutType(request.WorkoutDto.Type));
        }

        var workoutId = Guid.NewGuid();
        var workout = new Workout
        {
            Id = workoutId.ToString(),
            UserId = request.UserId.ToString(),
            Title = request.WorkoutDto.Title,
            Type = workoutType,
            Duration = request.WorkoutDto.Duration,
            CaloriesBurned = request.WorkoutDto.CaloriesBurned,
            WorkoutDate = request.WorkoutDto.WorkoutDate,
            CreatedAt = DateTime.UtcNow
        };

        var addResult = await repository.AddAsync(workout, ct);

        if (addResult.IsFailure)
        {
            return Result.Failure<CreateWorkoutResponse, Error>(addResult.Error);
        }

        var response = new CreateWorkoutResponse(
            Guid.Parse(workout.Id),
            Guid.Parse(workout.UserId),
            workout.Title,
            workout.Type.ToString(),
            workout.Duration,
            workout.CaloriesBurned,
            workout.WorkoutDate,
            workout.CreatedAt);

        return Result.Success<CreateWorkoutResponse, Error>(response);
    }

    public async Task<Result<WorkoutResponse, Error>> UpdateAsync(
        UpdateWorkoutRequest request,
        CancellationToken ct)
    {
        if (request.UserId == Guid.Empty)
        {
            return Result.Failure<WorkoutResponse, Error>(WorkoutErrors.UserIdRequired());
        }

        var workoutValidationError = ValidateWorkoutFields(
            request.WorkoutDto.Title,
            request.WorkoutDto.Type,
            request.WorkoutDto.Duration,
            request.WorkoutDto.CaloriesBurned,
            request.WorkoutDto.WorkoutDate);
        if (workoutValidationError is not null)
        {
            return Result.Failure<WorkoutResponse, Error>(workoutValidationError);
        }

        if (!Enum.TryParse<WorkoutType>(request.WorkoutDto.Type, true, out var workoutType))
        {
            return Result.Failure<WorkoutResponse, Error>(WorkoutErrors.InvalidWorkoutType(request.WorkoutDto.Type));
        }

        var workoutResult = await repository.GetByIdAsync(request.WorkoutId, ct);
        if (workoutResult.IsFailure)
        {
            return Result.Failure<WorkoutResponse, Error>(workoutResult.Error);
        }

        var workout = workoutResult.Value;

        workout.Title = request.WorkoutDto.Title;
        workout.Type = workoutType;
        workout.Duration = request.WorkoutDto.Duration;
        workout.CaloriesBurned = request.WorkoutDto.CaloriesBurned;
        workout.WorkoutDate = request.WorkoutDto.WorkoutDate;

        var updateResult = await repository.UpdateAsync(workout, ct);

        if (updateResult.IsFailure)
        {
            return Result.Failure<WorkoutResponse, Error>(updateResult.Error);
        }
        
        return Result.Success<WorkoutResponse, Error>(MapToResponse(workout));
    }
    
    public async Task<Result<WorkoutResponse, Error>> PatchAsync(
        PatchWorkoutRequest request,
        CancellationToken ct)
    {
        var workoutResult = await repository.GetByIdAsync(request.WorkoutId, ct);
        if (workoutResult.IsFailure)
        {
            return Result.Failure<WorkoutResponse, Error>(workoutResult.Error);
        }

        var workout = workoutResult.Value;
        
        var currentDto = new MergePatchWorkoutDto()
        {
            Title = workout.Title,
            Type = workout.Type.ToString(),
            Duration = workout.Duration,
            CaloriesBurned = workout.CaloriesBurned,
            WorkoutDate = workout.WorkoutDate
        };
        var patchedDto = mergePatchHelper.ApplyMergePatch(currentDto, request.Patch);

        var title = patchedDto.Title;
        var type = patchedDto.Type;
        var duration = patchedDto.Duration;
        var caloriesBurned = patchedDto.CaloriesBurned;
        var workoutDate = patchedDto.WorkoutDate;

        if (string.IsNullOrWhiteSpace(title))
        {
            return Result.Failure<WorkoutResponse, Error>(WorkoutErrors.TitleRequired());
        }

        if (string.IsNullOrWhiteSpace(type))
        {
            return Result.Failure<WorkoutResponse, Error>(WorkoutErrors.TypeRequired());
        }

        if (duration is null || duration.Value <= TimeSpan.Zero)
        {
            return Result.Failure<WorkoutResponse, Error>(WorkoutErrors.DurationMustBePositive());
        }

        if (caloriesBurned is null or < 0)
        {
            return Result.Failure<WorkoutResponse, Error>(WorkoutErrors.CaloriesBurnedMustBeNonNegative());
        }

        if (workoutDate is null || workoutDate.Value == default)
        {
            return Result.Failure<WorkoutResponse, Error>(WorkoutErrors.WorkoutDateRequired());
        }
        
        if (!Enum.TryParse<WorkoutType>(type, true, out var workoutType))
        {
            return Result.Failure<WorkoutResponse, Error>(WorkoutErrors.InvalidWorkoutType(type));
        }

        workout.Title = title;
        workout.Type = workoutType;
        workout.Duration = duration.Value;
        workout.CaloriesBurned = caloriesBurned.Value;
        workout.WorkoutDate = workoutDate.Value;

        var updateResult = await repository.UpdateAsync(workout, ct);

        if (updateResult.IsFailure)
        {
            return Result.Failure<WorkoutResponse, Error>(updateResult.Error);
        }

        return Result.Success<WorkoutResponse, Error>(MapToResponse(workout));
    }

    public async Task<UnitResult<Error>> DeleteAsync(DeleteWorkoutRequest request, CancellationToken ct)
    {
        return await repository.DeleteAsync(request.WorkoutId, ct);
    }

    public async Task<Result<WorkoutListResponse, Error>> GetByUserIdAsync(GetWorkoutsByUserIdRequest request,
        WorkoutFilter filter,
        SortParameters sortParameters,
        PageParameters pageParameters,
        CancellationToken ct)
    {
        var workoutsResult = await repository.GetByUserIdAsync(
            request.UserId,
            filter,
            sortParameters,
            pageParameters,
            ct);
        if (workoutsResult.IsFailure)
        {
            return Result.Failure<WorkoutListResponse, Error>(workoutsResult.Error);
        }
        
        var totalResult = await repository.GetCountByUserIdAsync(request.UserId, ct);
        if (totalResult.IsFailure)
        {
            return Result.Failure<WorkoutListResponse, Error>(totalResult.Error);
        }
        var total = totalResult.Value;

        var responses = workoutsResult.Value.Select(MapToResponse).ToList();
        return Result.Success<WorkoutListResponse, Error>(new WorkoutListResponse(responses, total));
    }

    public async Task<Result<WorkoutExercisesResponse, Error>> GetExercisesByWorkoutIdAsync(
        GetExercisesByWorkoutIdRequest request,
        CancellationToken ct)
    {
        var exercisesResult = await repository.GetExercisesByWorkoutIdAsync(request.WorkoutId, ct);
        if (exercisesResult.IsFailure)
        {
            return Result.Failure<WorkoutExercisesResponse, Error>(exercisesResult.Error);
        }

        var exerciseResponses = exercisesResult.Value.Select(MapExerciseToResponse).ToList();
        return Result.Success<WorkoutExercisesResponse, Error>(new WorkoutExercisesResponse(exerciseResponses));
    }

    public async Task<Result<ExerciseResponse, Error>> AddExerciseAsync(
        AddExerciseRequest request,
        CancellationToken ct)
    {
        var exerciseValidationError = ValidateExerciseFields(request.ExerciseDto.Name, request.ExerciseDto.Sets);
        if (exerciseValidationError is not null)
        {
            return Result.Failure<ExerciseResponse, Error>(exerciseValidationError);
        }

        var exercise = new Exercise
        {
            Name = request.ExerciseDto.Name,
            Sets = MapSetDtos(request.ExerciseDto.Sets)
        };

        var addResult = await repository.AddExerciseAsync(request.WorkoutId, exercise, ct);
        if (addResult.IsFailure)
        {
            return Result.Failure<ExerciseResponse, Error>(addResult.Error);
        }

        return Result.Success<ExerciseResponse, Error>(MapExerciseToResponse(exercise));
    }

    public async Task<Result<ExerciseResponse, Error>> UpdateExerciseAsync(
        UpdateExerciseRequest request,
        CancellationToken ct)
    {
        var exerciseValidationError = ValidateExerciseFields(request.ExerciseDto.Name, request.ExerciseDto.Sets);
        if (exerciseValidationError is not null)
        {
            return Result.Failure<ExerciseResponse, Error>(exerciseValidationError);
        }

        var exercise = new Exercise
        {
            Name = request.ExerciseDto.Name,
            Sets = MapSetDtos(request.ExerciseDto.Sets)
        };

        var updateResult = await repository.UpdateExerciseAsync(request.WorkoutId, request.ExerciseName, exercise, ct);
        if (updateResult.IsFailure)
        {
            return Result.Failure<ExerciseResponse, Error>(updateResult.Error);
        }

        return Result.Success<ExerciseResponse, Error>(MapExerciseToResponse(exercise));
    }

    public async Task<Result<ExerciseResponse, Error>> PatchExerciseAsync(
        PatchExerciseRequest request,
        CancellationToken ct)
    {
        var workoutResult = await repository.GetByIdAsync(request.WorkoutId, ct);
        if (workoutResult.IsFailure)
        {
            return Result.Failure<ExerciseResponse, Error>(workoutResult.Error);
        }

        var workout = workoutResult.Value;
        var exercise = workout.Exercises.FirstOrDefault(e =>
            string.Equals(e.Name, request.ExerciseName, StringComparison.OrdinalIgnoreCase));
        if (exercise is null)
        {
            return Result.Failure<ExerciseResponse, Error>(WorkoutErrors.ExerciseNotFound(request.WorkoutId, request.ExerciseName));
        }

        var currentDto = new MergePatchExerciseDto
        {
            Name = exercise.Name,
            Sets = exercise.Sets.Select(s => new AddSetDto(s.Reps, s.Weight)).ToList()
        };

        var patchedDto = mergePatchHelper.ApplyMergePatch(currentDto, request.Patch);

        var patchedName = patchedDto.Name;
        var patchedSets = patchedDto.Sets;

        var exerciseValidationError = ValidateExerciseFields(patchedName, patchedSets);
        if (exerciseValidationError is not null)
        {
            return Result.Failure<ExerciseResponse, Error>(exerciseValidationError);
        }

        var hasNameConflict = workout.Exercises.Any(e =>
            !ReferenceEquals(e, exercise) &&
            string.Equals(e.Name, patchedName, StringComparison.OrdinalIgnoreCase));
        if (hasNameConflict)
        {
            return Result.Failure<ExerciseResponse, Error>(WorkoutErrors.ExerciseAlreadyExists(request.WorkoutId, patchedName!));
        }

        exercise.Name = patchedName!;
        exercise.Sets = patchedSets!.Select(s => new Set { Reps = s.Reps, Weight = s.Weight }).ToList();

        var updateResult = await repository.UpdateAsync(workout, ct);
        if (updateResult.IsFailure)
        {
            return Result.Failure<ExerciseResponse, Error>(updateResult.Error);
        }

        return Result.Success<ExerciseResponse, Error>(MapExerciseToResponse(exercise));
    }

    public async Task<Result<WorkoutExercisesResponse, Error>> UpdateExercisesAsync(
        UpdateExercisesRequest request,
        CancellationToken ct)
    {
        foreach (var exerciseDto in request.ExerciseDtos.Exercises)
        {
            var exerciseValidationError = ValidateExerciseFields(exerciseDto.Name, exerciseDto.Sets);
            if (exerciseValidationError is not null)
            {
                return Result.Failure<WorkoutExercisesResponse, Error>(exerciseValidationError);
            }
        }

        var exercises = request.ExerciseDtos.Exercises.Select(MapExerciseDtoToDomain).ToList();

        var updateResult = await repository.UpdateExercisesAsync(request.WorkoutId, exercises, ct);
        if (updateResult.IsFailure)
        {
            return Result.Failure<WorkoutExercisesResponse, Error>(updateResult.Error);
        }

        var response = exercises.Select(MapExerciseToResponse).ToList();
        return Result.Success<WorkoutExercisesResponse, Error>(new WorkoutExercisesResponse(response));
    }

    public async Task<UnitResult<Error>> DeleteExerciseAsync(DeleteExerciseRequest request, CancellationToken ct)
    {
        return await repository.DeleteExerciseAsync(request.WorkoutId, request.ExerciseName, ct);
    }

    public async Task<Result<SetResponse, Error>> AddSetAsync(AddSetRequest request, CancellationToken ct)
    {
        var setValidationError = ValidateSetFields(request.SetDto.Reps, request.SetDto.Weight);
        if (setValidationError is not null)
        {
            return Result.Failure<SetResponse, Error>(setValidationError);
        }

        var set = new Set
        {
            Reps = request.SetDto.Reps,
            Weight = request.SetDto.Weight
        };

        var addResult = await repository.AddSetAsync(request.WorkoutId, request.ExerciseName, set, ct);
        if (addResult.IsFailure)
        {
            return Result.Failure<SetResponse, Error>(addResult.Error);
        }

        return Result.Success<SetResponse, Error>(new SetResponse(set.Reps, set.Weight));
    }

    public async Task<Result<SetResponse, Error>> UpdateSetAsync(UpdateSetRequest request, CancellationToken ct)
    {
        if (request.SetIndex < 0)
        {
            return Result.Failure<SetResponse, Error>(WorkoutErrors.InvalidSetIndex(request.SetIndex));
        }

        var setValidationError = ValidateSetFields(request.SetDto.Reps, request.SetDto.Weight);
        if (setValidationError is not null)
        {
            return Result.Failure<SetResponse, Error>(setValidationError);
        }

        var set = new Set
        {
            Reps = request.SetDto.Reps,
            Weight = request.SetDto.Weight
        };

        var updateResult = await repository.UpdateSetAsync(
            request.WorkoutId,
            request.ExerciseName,
            request.SetIndex,
            set,
            ct);
        if (updateResult.IsFailure)
        {
            return Result.Failure<SetResponse, Error>(updateResult.Error);
        }

        return Result.Success<SetResponse, Error>(new SetResponse(set.Reps, set.Weight));
    }

    public async Task<Result<SetResponse, Error>> PatchSetAsync(PatchSetRequest request, CancellationToken ct)
    {
        if (request.SetIndex < 0)
        {
            return Result.Failure<SetResponse, Error>(WorkoutErrors.InvalidSetIndex(request.SetIndex));
        }

        var workoutResult = await repository.GetByIdAsync(request.WorkoutId, ct);
        if (workoutResult.IsFailure)
        {
            return Result.Failure<SetResponse, Error>(workoutResult.Error);
        }

        var workout = workoutResult.Value;
        var exercise = workout.Exercises.FirstOrDefault(e =>
            string.Equals(e.Name, request.ExerciseName, StringComparison.OrdinalIgnoreCase));
        if (exercise is null)
        {
            return Result.Failure<SetResponse, Error>(WorkoutErrors.ExerciseNotFound(request.WorkoutId, request.ExerciseName));
        }

        if (request.SetIndex >= exercise.Sets.Count)
        {
            return Result.Failure<SetResponse, Error>(WorkoutErrors.SetNotFound(request.WorkoutId, request.ExerciseName, request.SetIndex));
        }

        var existingSet = exercise.Sets[request.SetIndex];
        var currentDto = new MergePatchSetDto
        {
            Reps = existingSet.Reps,
            Weight = existingSet.Weight
        };

        var patchedDto = mergePatchHelper.ApplyMergePatch(currentDto, request.Patch);
        if (!patchedDto.Reps.HasValue)
        {
            return Result.Failure<SetResponse, Error>(WorkoutErrors.SetRepsMustBePositive());
        }

        if (!patchedDto.Weight.HasValue)
        {
            return Result.Failure<SetResponse, Error>(WorkoutErrors.SetWeightMustBeNonNegative());
        }

        var setValidationError = ValidateSetFields(patchedDto.Reps.Value, patchedDto.Weight.Value);
        if (setValidationError is not null)
        {
            return Result.Failure<SetResponse, Error>(setValidationError);
        }

        existingSet.Reps = patchedDto.Reps.Value;
        existingSet.Weight = patchedDto.Weight.Value;

        var updateResult = await repository.UpdateAsync(workout, ct);
        if (updateResult.IsFailure)
        {
            return Result.Failure<SetResponse, Error>(updateResult.Error);
        }

        return Result.Success<SetResponse, Error>(new SetResponse(existingSet.Reps, existingSet.Weight));
    }

    public async Task<Result<SetListResponse, Error>> UpdateSetsAsync(UpdateSetsRequest request, CancellationToken ct)
    {
        foreach (var setDto in request.SetDtos.Sets)
        {
            var setValidationError = ValidateSetFields(setDto.Reps, setDto.Weight);
            if (setValidationError is not null)
            {
                return Result.Failure<SetListResponse, Error>(setValidationError);
            }
        }

        var sets = request.SetDtos.Sets.Select(s => new Set
        {
            Reps = s.Reps,
            Weight = s.Weight
        }).ToList();

        var updateResult = await repository.UpdateSetsAsync(
            request.WorkoutId,
            request.ExerciseName,
            sets,
            ct);
        if (updateResult.IsFailure)
        {
            return Result.Failure<SetListResponse, Error>(updateResult.Error);
        }

        return Result.Success<SetListResponse, Error>(
            new SetListResponse(sets.Select(s => new SetResponse(s.Reps, s.Weight)).ToList()));
    }

    public async Task<UnitResult<Error>> DeleteSetAsync(DeleteSetRequest request, CancellationToken ct)
    {
        if (request.SetIndex < 0)
        {
            return UnitResult.Failure(WorkoutErrors.InvalidSetIndex(request.SetIndex));
        }

        return await repository.DeleteSetAsync(request.WorkoutId, request.ExerciseName, request.SetIndex, ct);
    }

    public async Task<Result<AddPhotosToWorkoutResponse, Error>> AddPhotosToWorkoutAsync(
        AddPhotosToWorkoutRequest request,
        CancellationToken ct)
    {
        var photoIdResult = await repository.AddPhotosToWorkoutAsync(request.WorkoutId, ct);

        if (photoIdResult.IsFailure)
        {
            return Result.Failure<AddPhotosToWorkoutResponse, Error>(photoIdResult.Error);
        }

        return new AddPhotosToWorkoutResponse(photoIdResult.Value);
    }
    
    

    private static WorkoutResponse MapToResponse(Workout workout)
    {
        return new WorkoutResponse(
            Guid.Parse(workout.Id),
            Guid.Parse(workout.UserId),
            workout.Title,
            workout.Type.ToString(),
            workout.Duration,
            workout.CaloriesBurned,
            workout.WorkoutDate,
            workout.CreatedAt);
    }

    private static ExerciseResponse MapExerciseToResponse(Exercise exercise)
    {
        var setResponses = exercise.Sets
            .Select(s => new SetResponse(s.Reps, s.Weight))
            .ToList();

        return new ExerciseResponse(exercise.Name, setResponses);
    }
    

    private static Exercise MapExerciseDtoToDomain(UpdateExerciseDto dto)
    {
        return new Exercise
        {
            Name = dto.Name,
            Sets = MapSetDtos(dto.Sets)
        };
    }

    private static List<Set> MapSetDtos(IReadOnlyList<SetDto> setDtos)
    {
        return setDtos.Select(s => new Set
        {
            Reps = s.Reps,
            Weight = s.Weight
        }).ToList();
    }

    private static Error? ValidateWorkoutFields(
        string? title,
        string? type,
        TimeSpan duration,
        int caloriesBurned,
        DateTime workoutDate)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return WorkoutErrors.TitleRequired();
        }

        if (string.IsNullOrWhiteSpace(type))
        {
            return WorkoutErrors.TypeRequired();
        }

        if (duration <= TimeSpan.Zero)
        {
            return WorkoutErrors.DurationMustBePositive();
        }

        if (caloriesBurned < 0)
        {
            return WorkoutErrors.CaloriesBurnedMustBeNonNegative();
        }

        if (workoutDate == default)
        {
            return WorkoutErrors.WorkoutDateRequired();
        }

        return null;
    }

    private static Error? ValidateExerciseFields(string? name, IReadOnlyList<SetDto>? sets)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return WorkoutErrors.ExerciseNameRequired();
        }

        if (sets is null)
        {
            return WorkoutErrors.ExerciseSetsRequired();
        }

        foreach (var set in sets)
        {
            var setValidationError = ValidateSetFields(set.Reps, set.Weight);
            if (setValidationError is not null)
            {
                return setValidationError;
            }
        }

        return null;
    }

    private static Error? ValidateExerciseFields(string? name, IReadOnlyList<AddSetDto>? sets)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return WorkoutErrors.ExerciseNameRequired();
        }

        if (sets is null)
        {
            return WorkoutErrors.ExerciseSetsRequired();
        }

        foreach (var set in sets)
        {
            var setValidationError = ValidateSetFields(set.Reps, set.Weight);
            if (setValidationError is not null)
            {
                return setValidationError;
            }
        }

        return null;
    }

    private static Error? ValidateSetFields(int reps, double weight)
    {
        if (reps <= 0)
        {
            return WorkoutErrors.SetRepsMustBePositive();
        }

        if (weight < 0)
        {
            return WorkoutErrors.SetWeightMustBeNonNegative();
        }

        return null;
    }
}