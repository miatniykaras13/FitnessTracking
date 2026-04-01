using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Features.Commands.AddSet;
using FitnessTracking.Domain.Enums;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Contracts;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace FitnessTracking.Application.Tests.Features.Commands;

public class AddSetCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Add_Set_When_Request_Is_Valid()
    {
        var repository = new Mock<IWorkoutsRepository>();
        var validator = new Mock<IValidator<AddSetCommand>>();
        var userId = Guid.NewGuid();
        var workoutId = Guid.NewGuid();
        var command = new AddSetCommand(workoutId, userId, "Bench", new AddSetDto(10, 80));

        validator.Setup(x => x.ValidateAsync(It.IsAny<AddSetCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        repository.Setup(x => x.GetByIdAsync(workoutId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateWorkout(userId, workoutId));

        repository.Setup(x => x.AddSetAsync(workoutId, "Bench", It.IsAny<Set>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid _, string _, Set s, CancellationToken _) => s);

        var handler = new AddSetCommandHandler(repository.Object, validator.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(10, result.Value.Reps);
        Assert.Equal(80, result.Value.Weight);
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
        Exercises = [new Exercise { Name = "Bench", Sets = [] }]
    };
}

