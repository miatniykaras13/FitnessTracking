using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Features.Commands.UpdateWorkout;
using FitnessTracking.Domain.Enums;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Contracts;
using FitnessTracking.Shared.Errors;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace FitnessTracking.Application.Tests.Features.Commands;

public class UpdateWorkoutCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Update_Workout_When_Request_Is_Valid()
    {
        var repository = new Mock<IWorkoutsRepository>();
        var validator = new Mock<IValidator<UpdateWorkoutCommand>>();
        var userId = Guid.NewGuid();
        var workoutId = Guid.NewGuid();
        var dto = new UpdateWorkoutDto("Updated", "HIIT", TimeSpan.FromMinutes(25), 350, new DateTime(2026, 3, 26));
        var command = new UpdateWorkoutCommand(workoutId, userId, dto);
        var workout = CreateWorkout(userId, workoutId);

        validator.Setup(x => x.ValidateAsync(It.IsAny<UpdateWorkoutCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        repository.Setup(x => x.GetByIdAsync(workoutId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Workout, Error>(workout));
        repository.Setup(x => x.UpdateAsync(workout, It.IsAny<CancellationToken>()))
            .ReturnsAsync(UnitResult.Success<Error>());

        var handler = new UpdateWorkoutCommandHandler(repository.Object, validator.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Updated", result.Value.Title);
        Assert.Equal("HIIT", result.Value.Type);
    }

    private static Workout CreateWorkout(Guid userId, Guid workoutId) => new()
    {
        Id = workoutId.ToString(),
        UserId = userId.ToString(),
        Title = "Old",
        Type = WorkoutType.Strength,
        Duration = TimeSpan.FromMinutes(40),
        CaloriesBurned = 300,
        WorkoutDate = new DateTime(2026, 3, 20),
        CreatedAt = new DateTime(2026, 3, 1)
    };
}

