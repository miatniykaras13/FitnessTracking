using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Features.Queries.GetExercisesByWorkoutId;
using FitnessTracking.Domain.Enums;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Errors;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace FitnessTracking.Application.Tests.Features.Queries;

public class GetExercisesByWorkoutIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_Should_Return_Exercises_When_Workout_Exists()
    {
        var repository = new Mock<IWorkoutsRepository>();
        var validator = new Mock<IValidator<GetExercisesByWorkoutIdQuery>>();
        var workoutId = Guid.NewGuid();
        var query = new GetExercisesByWorkoutIdQuery(workoutId);
        var workout = CreateWorkout(workoutId, Guid.NewGuid());
        var exercises = new List<Exercise>
        {
            new() { Name = "Bench", Sets = [new Set { Reps = 10, Weight = 80 }] }
        };

        validator.Setup(x => x.ValidateAsync(It.IsAny<GetExercisesByWorkoutIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        repository.Setup(x => x.GetByIdAsync(workoutId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(workout);
        repository.Setup(x => x.GetExercisesByWorkoutIdAsync(workoutId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exercises);

        var handler = new GetExercisesByWorkoutIdQueryHandler(repository.Object, validator.Object);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value.Exercises);
    }

    private static Workout CreateWorkout(Guid workoutId, Guid userId) => new()
    {
        Id = workoutId.ToString(),
        UserId = userId.ToString(),
        Title = "Workout",
        Type = WorkoutType.Strength,
        Duration = TimeSpan.FromMinutes(45),
        CaloriesBurned = 350,
        WorkoutDate = new DateTime(2026, 3, 26),
        CreatedAt = new DateTime(2026, 3, 1)
    };
}

