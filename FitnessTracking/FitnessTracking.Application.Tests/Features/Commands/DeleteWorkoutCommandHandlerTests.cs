using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Features.Commands.DeleteWorkout;
using FitnessTracking.Domain.Enums;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Errors;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace FitnessTracking.Application.Tests.Features.Commands;

public class DeleteWorkoutCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Delete_Workout_When_Request_Is_Valid()
    {
        var repository = new Mock<IWorkoutsRepository>();
        var validator = new Mock<IValidator<DeleteWorkoutCommand>>();
        var userId = Guid.NewGuid();
        var workoutId = Guid.NewGuid();
        var command = new DeleteWorkoutCommand(workoutId, userId);

        validator.Setup(x => x.ValidateAsync(It.IsAny<DeleteWorkoutCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        repository.Setup(x => x.GetByIdAsync(workoutId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Workout, Error>(CreateWorkout(userId, workoutId)));
        repository.Setup(x => x.DeleteAsync(workoutId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(UnitResult.Success<Error>());

        var handler = new DeleteWorkoutCommandHandler(repository.Object, validator.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        repository.Verify(x => x.DeleteAsync(workoutId, It.IsAny<CancellationToken>()), Times.Once);
    }

    private static Workout CreateWorkout(Guid userId, Guid workoutId) => new()
    {
        Id = workoutId.ToString(),
        UserId = userId.ToString(),
        Title = "Workout",
        Type = WorkoutType.Strength,
        Duration = TimeSpan.FromMinutes(40),
        CaloriesBurned = 300,
        WorkoutDate = new DateTime(2026, 3, 26),
        CreatedAt = new DateTime(2026, 3, 1)
    };
}

