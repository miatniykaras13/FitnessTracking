using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Features.Commands.DeleteExercise;
using FitnessTracking.Domain.Enums;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Errors;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace FitnessTracking.Application.Tests.Features.Commands;

public class DeleteExerciseCommandHandlerTests
{
    private readonly Mock<IWorkoutsRepository> _repository = new();
    private readonly Mock<IValidator<DeleteExerciseCommand>> _validator = new();

    [Fact]
    public async Task Handle_Should_Return_NotFound_When_Workout_Does_Not_Exist()
    {
        var userId = Guid.NewGuid();
        var workoutId = Guid.NewGuid();
        var command = new DeleteExerciseCommand(userId, workoutId, "Squat");

        _validator
            .Setup(x => x.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _repository
            .Setup(x => x.GetByIdAsync(workoutId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<Workout, Error>(WorkoutErrors.WorkoutNotFound(workoutId)));

        var handler = new DeleteExerciseCommandHandler(_repository.Object, _validator.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Error, x => x.Code == "workout.not_found");

        _repository.Verify(
            x => x.DeleteExerciseAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Return_Forbidden_When_Workout_Belongs_To_Another_User()
    {
        var userId = Guid.NewGuid();
        var workoutId = Guid.NewGuid();
        var command = new DeleteExerciseCommand(userId, workoutId, "Squat");
        var workout = CreateWorkout(Guid.NewGuid(), workoutId);

        _validator
            .Setup(x => x.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _repository
            .Setup(x => x.GetByIdAsync(workoutId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Workout, Error>(workout));

        var handler = new DeleteExerciseCommandHandler(_repository.Object, _validator.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Error, x => x.Code == "workout.is_forbidden");

        _repository.Verify(
            x => x.DeleteExerciseAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Delete_Exercise_When_Request_Is_Valid()
    {
        var userId = Guid.NewGuid();
        var workoutId = Guid.NewGuid();
        var command = new DeleteExerciseCommand(userId, workoutId, "Squat");
        var workout = CreateWorkout(userId, workoutId);

        _validator
            .Setup(x => x.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _repository
            .Setup(x => x.GetByIdAsync(workoutId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Workout, Error>(workout));

        _repository
            .Setup(x => x.DeleteExerciseAsync(workoutId, "Squat", It.IsAny<CancellationToken>()))
            .ReturnsAsync(UnitResult.Success<Error>());

        var handler = new DeleteExerciseCommandHandler(_repository.Object, _validator.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        _repository.Verify(x => x.DeleteExerciseAsync(workoutId, "Squat", It.IsAny<CancellationToken>()), Times.Once);
    }

    private static Workout CreateWorkout(Guid userId, Guid workoutId)
    {
        return new Workout
        {
            Id = workoutId.ToString(),
            UserId = userId.ToString(),
            Title = "Lower Body",
            Type = WorkoutType.Strength,
            Duration = TimeSpan.FromMinutes(45),
            CaloriesBurned = 380,
            WorkoutDate = new DateTime(2026, 3, 26),
            CreatedAt = new DateTime(2026, 3, 1)
        };
    }
}

