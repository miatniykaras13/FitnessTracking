using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Features.Commands.UpdateSets;
using FitnessTracking.Domain.Enums;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Contracts;
using FitnessTracking.Shared.Errors;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace FitnessTracking.Application.Tests.Features.Commands;

public class UpdateSetsCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Update_Sets_When_Request_Is_Valid()
    {
        var repository = new Mock<IWorkoutsRepository>();
        var validator = new Mock<IValidator<UpdateSetsCommand>>();
        var userId = Guid.NewGuid();
        var workoutId = Guid.NewGuid();
        var dto = new UpdateSetsDto([new UpdateSetDto(10, 100), new UpdateSetDto(8, 110)]);
        var command = new UpdateSetsCommand(userId, workoutId, "Bench", dto);

        validator.Setup(x => x.ValidateAsync(It.IsAny<UpdateSetsCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        repository.Setup(x => x.GetByIdAsync(workoutId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Workout, Error>(CreateWorkout(userId, workoutId)));
        repository.Setup(x => x.UpdateSetsAsync(workoutId, "Bench", It.IsAny<IReadOnlyList<Set>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new UpdateSetsCommandHandler(repository.Object, validator.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Sets.Count());
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
        Exercises = [new Exercise { Name = "Bench", Sets = [new Set { Reps = 10, Weight = 90 }] }]
    };
}

