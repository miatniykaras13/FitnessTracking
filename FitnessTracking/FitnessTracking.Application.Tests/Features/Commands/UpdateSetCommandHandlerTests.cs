using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Features.Commands.UpdateSet;
using FitnessTracking.Domain.Enums;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Contracts;
using FitnessTracking.Shared.Errors;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace FitnessTracking.Application.Tests.Features.Commands;

public class UpdateSetCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Update_Set_When_Request_Is_Valid()
    {
        var repository = new Mock<IWorkoutsRepository>();
        var validator = new Mock<IValidator<UpdateSetCommand>>();
        var userId = Guid.NewGuid();
        var workoutId = Guid.NewGuid();
        var command = new UpdateSetCommand(userId, workoutId, "Bench", 0, new UpdateSetDto(9, 90));

        validator.Setup(x => x.ValidateAsync(It.IsAny<UpdateSetCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        repository.Setup(x => x.GetByIdAsync(workoutId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateWorkout(userId, workoutId));
        repository.Setup(x => x.UpdateSetAsync(workoutId, "Bench", 0, It.IsAny<Set>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new UpdateSetCommandHandler(repository.Object, validator.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(9, result.Value.Reps);
        Assert.Equal(90, result.Value.Weight);
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
        CreatedAt = new DateTime(2026, 3, 1),
        Exercises =
        [
            new Exercise
            {
                Name = "Bench",
                Sets = [new Set { Reps = 8, Weight = 80 }]
            }
        ]
    };
}

