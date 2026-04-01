using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Features.Commands.AddExercise;
using FitnessTracking.Domain.Enums;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Contracts;
using FitnessTracking.Shared.Errors;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace FitnessTracking.Application.Tests.Features.Commands;

public class AddExerciseCommandHandlerTests
{
    private readonly Mock<IWorkoutsRepository> _repository = new();
    private readonly Mock<IValidator<AddExerciseCommand>> _validator = new();

    [Fact]
    public async Task Handle_Should_Return_Forbidden_When_Workout_Belongs_To_Another_User()
    {
        var userId = Guid.NewGuid();
        var workoutId = Guid.NewGuid();
        var command = new AddExerciseCommand(userId, workoutId, BuildExerciseDto());
        var workout = CreateWorkout(Guid.NewGuid(), workoutId);

        _validator
            .Setup(x => x.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _repository
            .Setup(x => x.GetByIdAsync(workoutId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(workout);

        var handler = new AddExerciseCommandHandler(_repository.Object, _validator.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Error, x => x.Code == "workout.is_forbidden");
        _repository.Verify(
            x => x.AddExerciseAsync(It.IsAny<Guid>(), It.IsAny<Exercise>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Return_Conflict_When_Repository_Detects_Duplicate_Exercise()
    {
        var userId = Guid.NewGuid();
        var workoutId = Guid.NewGuid();
        var command = new AddExerciseCommand(userId, workoutId, BuildExerciseDto());
        var workout = CreateWorkout(userId, workoutId);

        _validator
            .Setup(x => x.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _repository
            .Setup(x => x.GetByIdAsync(workoutId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(workout);

        _repository
            .Setup(x => x.AddExerciseAsync(workoutId, It.IsAny<Exercise>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(UnitResult.Failure<Error>(WorkoutErrors.ExerciseAlreadyExists(workoutId, command.ExerciseDto.Name)));

        var handler = new AddExerciseCommandHandler(_repository.Object, _validator.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Error, x => x.Code == "workout.exercise.is_conflict");
    }

    [Fact]
    public async Task Handle_Should_Add_Exercise_And_Return_Response_When_Request_Is_Valid()
    {
        var userId = Guid.NewGuid();
        var workoutId = Guid.NewGuid();
        var dto = BuildExerciseDto();
        var command = new AddExerciseCommand(userId, workoutId, dto);
        var workout = CreateWorkout(userId, workoutId);

        _validator
            .Setup(x => x.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _repository
            .Setup(x => x.GetByIdAsync(workoutId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(workout);

        _repository
            .Setup(x => x.AddExerciseAsync(
                workoutId,
                It.Is<Exercise>(e =>
                    e.Name == dto.Name &&
                    e.Sets.Count == 2 &&
                    e.Sets[0].Reps == 10 && e.Sets[0].Weight.Equals(60) &&
                    e.Sets[1].Reps == 8 && e.Sets[1].Weight.Equals(70)),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(UnitResult.Success<Error>());

        var handler = new AddExerciseCommandHandler(_repository.Object, _validator.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Bench Press", result.Value.Name);
        Assert.Collection(
            result.Value.Sets,
            s =>
            {
                Assert.Equal(10, s.Reps);
                Assert.Equal(60, s.Weight);
            },
            s =>
            {
                Assert.Equal(8, s.Reps);
                Assert.Equal(70, s.Weight);
            });
    }

    private static AddExerciseDto BuildExerciseDto() =>
        new("Bench Press", [new AddSetDto(10, 60), new AddSetDto(8, 70)]);

    private static Workout CreateWorkout(Guid userId, Guid workoutId)
    {
        return new Workout
        {
            Id = workoutId.ToString(),
            UserId = userId.ToString(),
            Title = "Upper Body",
            Type = WorkoutType.Strength,
            Duration = TimeSpan.FromMinutes(50),
            CaloriesBurned = 320,
            WorkoutDate = new DateTime(2026, 3, 26),
            CreatedAt = new DateTime(2026, 3, 1)
        };
    }
}

