using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Features.Queries.GetWorkoutById;
using FitnessTracking.Domain.Enums;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Errors;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace FitnessTracking.Application.Tests.Features.Queries;

public class GetWorkoutByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_Should_Return_Workout_When_Found()
    {
        var repository = new Mock<IWorkoutsRepository>();
        var validator = new Mock<IValidator<GetWorkoutByIdQuery>>();
        var workoutId = Guid.NewGuid();
        var query = new GetWorkoutByIdQuery(workoutId);
        var workout = CreateWorkout(workoutId, Guid.NewGuid());

        validator.Setup(x => x.ValidateAsync(It.IsAny<GetWorkoutByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        repository.Setup(x => x.GetByIdWithPhotosAsync(workoutId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(workout);

        var handler = new GetWorkoutByIdQueryHandler(repository.Object, validator.Object);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(workoutId, result.Value.WorkoutId);
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
        CreatedAt = new DateTime(2026, 3, 1),
        ProgressPhotos = [new WorkoutPhoto { Id = Guid.NewGuid().ToString(), WorkoutId = workoutId.ToString(), Path = "uploads/p.jpg", CreatedAt = new DateTime(2026, 3, 20) }]
    };
}

