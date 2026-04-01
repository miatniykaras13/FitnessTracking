using System.Text.Json.Nodes;
using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.Helpers;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Features.Commands.PatchWorkout;
using FitnessTracking.Domain.Enums;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Contracts;
using FitnessTracking.Shared.Errors;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace FitnessTracking.Application.Tests.Features.Commands;

public class PatchWorkoutCommandHandlerTests
{
    private readonly Mock<IWorkoutsRepository> _repository = new();
    private readonly Mock<IMergePatchHelper> _mergePatchHelper = new();
    private readonly Mock<IValidator<PatchWorkoutCommand>> _commandValidator = new();
    private readonly Mock<IValidator<MergePatchWorkoutDto>> _patchDtoValidator = new();

    [Fact]
    public async Task Handle_Should_Return_Forbidden_When_Workout_Belongs_To_Other_User()
    {
        var requestUserId = Guid.NewGuid();
        var workoutId = Guid.NewGuid();
        var command = new PatchWorkoutCommand(requestUserId, workoutId, new JsonObject());
        var workout = CreateWorkout(Guid.NewGuid(), workoutId);

        _commandValidator
            .Setup(x => x.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _repository
            .Setup(x => x.GetByIdAsync(workoutId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Workout, Error>(workout));

        var handler = new PatchWorkoutCommandHandler(
            _repository.Object,
            _mergePatchHelper.Object,
            _commandValidator.Object,
            _patchDtoValidator.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Error, x => x.Code == "workout.is_forbidden");

        _mergePatchHelper.Verify(
            x => x.ApplyMergePatch(It.IsAny<MergePatchWorkoutDto>(), It.IsAny<JsonObject>(), It.IsAny<System.Text.Json.JsonSerializerOptions?>()),
            Times.Never);

        _repository.Verify(x => x.UpdateAsync(It.IsAny<Workout>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Update_Workout_And_Return_Response_When_Patch_Is_Valid()
    {
        var userId = Guid.NewGuid();
        var workoutId = Guid.NewGuid();
        var command = new PatchWorkoutCommand(userId, workoutId, new JsonObject { ["title"] = "Updated title" });
        var workout = CreateWorkout(userId, workoutId);

        var patchedDto = new MergePatchWorkoutDto
        {
            Title = "Updated title",
            Type = "Cardio",
            Duration = TimeSpan.FromMinutes(30),
            CaloriesBurned = 420,
            WorkoutDate = new DateTime(2026, 3, 26)
        };

        _commandValidator
            .Setup(x => x.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _repository
            .Setup(x => x.GetByIdAsync(workoutId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Workout, Error>(workout));

        _mergePatchHelper
            .Setup(x => x.ApplyMergePatch(It.IsAny<MergePatchWorkoutDto>(), command.Patch, It.IsAny<System.Text.Json.JsonSerializerOptions?>()))
            .Returns(patchedDto);

        _patchDtoValidator
            .Setup(x => x.ValidateAsync(patchedDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _repository
            .Setup(x => x.UpdateAsync(workout, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new PatchWorkoutCommandHandler(
            _repository.Object,
            _mergePatchHelper.Object,
            _commandValidator.Object,
            _patchDtoValidator.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(workoutId, result.Value.WorkoutId);
        Assert.Equal(userId, result.Value.UserId);
        Assert.Equal("Updated title", result.Value.Title);
        Assert.Equal(WorkoutType.Cardio.ToString(), result.Value.Type);
        Assert.Equal(420, result.Value.CaloriesBurned);

        _repository.Verify(x => x.UpdateAsync(workout, It.IsAny<CancellationToken>()), Times.Once);
    }

    private static Workout CreateWorkout(Guid userId, Guid workoutId)
    {
        return new Workout
        {
            Id = workoutId.ToString(),
            UserId = userId.ToString(),
            Title = "Base title",
            Type = WorkoutType.Strength,
            Duration = TimeSpan.FromMinutes(60),
            CaloriesBurned = 300,
            WorkoutDate = new DateTime(2026, 1, 1),
            CreatedAt = new DateTime(2026, 1, 1)
        };
    }
}

