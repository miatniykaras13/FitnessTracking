using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Extensions;
using FitnessTracking.Domain.Enums;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Errors;
using FluentValidation;

namespace FitnessTracking.Application.Features.Commands.CreateWorkout;

public class CreateWorkoutCommandHandler(
    IWorkoutsRepository repository,
    IValidator<CreateWorkoutCommand> validator)
    : ICommandHandler<CreateWorkoutCommand, Result<CreateWorkoutResponse, List<Error>>>
{
    public async Task<Result<CreateWorkoutResponse, List<Error>>> Handle(CreateWorkoutCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.ToErrors(nameof(Workout).ToLower());
        }

        if (!Enum.TryParse<WorkoutType>(request.WorkoutDto.Type, true, out var workoutType))
        {
            return Result.Failure<CreateWorkoutResponse, List<Error>>(
                WorkoutErrors.InvalidWorkoutType(request.WorkoutDto.Type));
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

        var addedWorkout = await repository.AddAsync(workout, cancellationToken);

        var response = new CreateWorkoutResponse(
            Guid.Parse(addedWorkout.Id),
            Guid.Parse(addedWorkout.UserId),
            addedWorkout.Title,
            addedWorkout.Type.ToString(),
            addedWorkout.Duration,
            addedWorkout.CaloriesBurned,
            addedWorkout.WorkoutDate,
            addedWorkout.CreatedAt);

        return Result.Success<CreateWorkoutResponse, List<Error>>(response);
    }
}