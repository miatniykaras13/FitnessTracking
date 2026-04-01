using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Features.Commands.UpdateExercise;
using FitnessTracking.Domain.Enums;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Contracts;
using FitnessTracking.Shared.Errors;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace FitnessTracking.Application.Tests.Features.Commands;

public class UpdateExerciseCommandHandlerTests
{
    private readonly Mock<IWorkoutsRepository> _repository = new();
    private readonly Mock<IValidator<UpdateExerciseCommand>> _validator = new();

    [Fact]
    public async Task Handle_Should_Return_Forbidden_When_Workout_Belongs_To_Another_User()
    {
        var userId = Guid.NewGuid();
        var workoutId = Guid.NewGuid();
        var command = new UpdateExerciseCommand(
            userId,
            workoutId,
            "Squat",
            new UpdateExerciseDto("Back Squat", [new SetDto(8, 70)]));
        var workout = CreateWorkout(Guid.NewGuid(), workoutId);

        _validator
            .Setup(x => x.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _repository
            .Setup(x => x.GetByIdAsync(workoutId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(workout);

        var handler = new UpdateExerciseCommandHandler(_repository.Object, _validator.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Error, x => x.Code == "workout.is_forbidden");
        _repository.Verify(
            x => x.UpdateExerciseAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Exercise>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Return_Failure_When_Repository_Update_Fails()
    {
        var userId = Guid.NewGuid();
        var workoutId = Guid.NewGuid();
        var command = new UpdateExerciseCommand(
            userId,
            workoutId,
            "Squat",
            new UpdateExerciseDto("Back Squat", [new SetDto(8, 70)]));
        var workout = CreateWorkout(userId, workoutId);

        _validator
            .Setup(x => x.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _repository
            .Setup(x => x.GetByIdAsync(workoutId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(workout);

        _repository
            .Setup(x => x.UpdateExerciseAsync(workoutId, "Squat", It.IsAny<Exercise>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new UpdateExerciseCommandHandler(_repository.Object, _validator.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Error, x => x.Code == "workout.exercise.not_found");
    }

    [Fact]
    public async Task Handle_Should_Map_Dto_And_Return_Response_When_Update_Succeeds()
    {
        var userId = Guid.NewGuid();
        var workoutId = Guid.NewGuid();
        var dto = new UpdateExerciseDto("Back Squat", [new SetDto(8, 70), new SetDto(6, 80)]);
        var command = new UpdateExerciseCommand(userId, workoutId, "Squat", dto);
        var workout = CreateWorkout(userId, workoutId);

        _validator
            .Setup(x => x.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _repository
            .Setup(x => x.GetByIdAsync(workoutId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(workout);

        _repository
            .Setup(x => x.UpdateExerciseAsync(
                workoutId,
                "Squat",
                It.Is<Exercise>(e =>
                    e.Name == "Back Squat" &&
                    e.Sets.Count == 2 &&
                    e.Sets[0].Reps == 8 && e.Sets[0].Weight.Equals(70) &&
                    e.Sets[1].Reps == 6 && e.Sets[1].Weight.Equals(80)),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new UpdateExerciseCommandHandler(_repository.Object, _validator.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Back Squat", result.Value.Name);
        Assert.Collection(
            result.Value.Sets,
            s =>
            {
                Assert.Equal(8, s.Reps);
                Assert.Equal(70, s.Weight);
            },
            s =>
            {
                Assert.Equal(6, s.Reps);
                Assert.Equal(80, s.Weight);
            });
    }

    private static Workout CreateWorkout(Guid userId, Guid workoutId)
    {
        return new Workout
        {
            Id = workoutId.ToString(),
            UserId = userId.ToString(),
            Title = "Workout",
            Type = WorkoutType.Strength,
            Duration = TimeSpan.FromMinutes(40),
            CaloriesBurned = 350,
            WorkoutDate = new DateTime(2026, 3, 20),
            CreatedAt = new DateTime(2026, 3, 1),
            Exercises = [new Exercise { Name = "Squat", Sets = [new Set { Reps = 10, Weight = 60 }] }]
        };
    }
}

