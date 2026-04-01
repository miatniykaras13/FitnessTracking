using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Features.Commands.UpdateExercises;
using FitnessTracking.Domain.Enums;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Contracts;
using FitnessTracking.Shared.Errors;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace FitnessTracking.Application.Tests.Features.Commands;

public class UpdateExercisesCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Update_Exercises_When_Request_Is_Valid()
    {
        var repository = new Mock<IWorkoutsRepository>();
        var validator = new Mock<IValidator<UpdateExercisesCommand>>();
        var userId = Guid.NewGuid();
        var workoutId = Guid.NewGuid();
        var dto = new UpdateExercisesDto([
            new UpdateExerciseDto("Bench", [new SetDto(10, 90)]),
            new UpdateExerciseDto("Row", [new SetDto(12, 70)])
        ]);
        var command = new UpdateExercisesCommand(userId, workoutId, dto);

        validator.Setup(x => x.ValidateAsync(It.IsAny<UpdateExercisesCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        repository.Setup(x => x.GetByIdAsync(workoutId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateWorkout(userId, workoutId));
        repository.Setup(x => x.UpdateExercisesAsync(workoutId, It.IsAny<IReadOnlyList<Exercise>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new UpdateExercisesCommandHandler(repository.Object, validator.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Exercises.Count());
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

