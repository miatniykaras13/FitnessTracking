using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Features.Commands.DeleteSet;
using FitnessTracking.Domain.Enums;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Errors;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace FitnessTracking.Application.Tests.Features.Commands;

public class DeleteSetCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Delete_Set_When_Request_Is_Valid()
    {
        var repository = new Mock<IWorkoutsRepository>();
        var validator = new Mock<IValidator<DeleteSetCommand>>();
        var userId = Guid.NewGuid();
        var workoutId = Guid.NewGuid();
        var command = new DeleteSetCommand(userId, workoutId, "Bench", 0);

        validator.Setup(x => x.ValidateAsync(It.IsAny<DeleteSetCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        repository.Setup(x => x.GetByIdAsync(workoutId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Workout, Error>(CreateWorkout(userId, workoutId)));
        repository.Setup(x => x.DeleteSetAsync(workoutId, "Bench", 0, It.IsAny<CancellationToken>()))
            .ReturnsAsync(UnitResult.Success<Error>());

        var handler = new DeleteSetCommandHandler(repository.Object, validator.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
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

