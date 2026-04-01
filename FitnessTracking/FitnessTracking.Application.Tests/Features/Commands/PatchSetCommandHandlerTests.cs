using System.Text.Json.Nodes;
using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.Helpers;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Features.Commands.PatchSet;
using FitnessTracking.Domain.Enums;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Contracts;
using FitnessTracking.Shared.Errors;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace FitnessTracking.Application.Tests.Features.Commands;

public class PatchSetCommandHandlerTests
{
    private readonly Mock<IWorkoutsRepository> _repository = new();
    private readonly Mock<IMergePatchHelper> _mergePatchHelper = new();
    private readonly Mock<IValidator<PatchSetCommand>> _commandValidator = new();
    private readonly Mock<IValidator<MergePatchSetDto>> _patchValidator = new();

    [Fact]
    public async Task Handle_Should_Return_Forbidden_When_Workout_Belongs_To_Another_User()
    {
        var requestUserId = Guid.NewGuid();
        var workoutId = Guid.NewGuid();
        var command = new PatchSetCommand(requestUserId, workoutId, "Squat", 0, new JsonObject());
        var workout = CreateWorkout(Guid.NewGuid(), workoutId);

        _commandValidator
            .Setup(x => x.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _repository
            .Setup(x => x.GetByIdAsync(workoutId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Workout, Error>(workout));

        var handler = new PatchSetCommandHandler(
            _repository.Object,
            _mergePatchHelper.Object,
            _commandValidator.Object,
            _patchValidator.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Error, x => x.Code == "workout.is_forbidden");

        _mergePatchHelper.Verify(
            x => x.ApplyMergePatch(It.IsAny<MergePatchSetDto>(), It.IsAny<JsonObject>(), It.IsAny<System.Text.Json.JsonSerializerOptions?>()),
            Times.Never);
        _repository.Verify(x => x.UpdateAsync(It.IsAny<Workout>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Return_NotFound_When_SetIndex_Is_Out_Of_Range()
    {
        var userId = Guid.NewGuid();
        var workoutId = Guid.NewGuid();
        var command = new PatchSetCommand(userId, workoutId, "Squat", 3, new JsonObject());
        var workout = CreateWorkout(userId, workoutId);

        _commandValidator
            .Setup(x => x.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _repository
            .Setup(x => x.GetByIdAsync(workoutId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Workout, Error>(workout));

        var handler = new PatchSetCommandHandler(
            _repository.Object,
            _mergePatchHelper.Object,
            _commandValidator.Object,
            _patchValidator.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Error, x => x.Code == "workout.exercise.set.not_found");

        _mergePatchHelper.Verify(
            x => x.ApplyMergePatch(It.IsAny<MergePatchSetDto>(), It.IsAny<JsonObject>(), It.IsAny<System.Text.Json.JsonSerializerOptions?>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Update_Set_And_Return_Response_When_Patch_Is_Valid()
    {
        var userId = Guid.NewGuid();
        var workoutId = Guid.NewGuid();
        var command = new PatchSetCommand(userId, workoutId, "Squat", 0, new JsonObject { ["reps"] = 12 });
        var workout = CreateWorkout(userId, workoutId);
        var patchedDto = new MergePatchSetDto
        {
            Reps = 12,
            Weight = 55
        };

        _commandValidator
            .Setup(x => x.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _repository
            .Setup(x => x.GetByIdAsync(workoutId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Workout, Error>(workout));

        _mergePatchHelper
            .Setup(x => x.ApplyMergePatch(It.IsAny<MergePatchSetDto>(), command.Patch, It.IsAny<System.Text.Json.JsonSerializerOptions?>()))
            .Returns(patchedDto);

        _patchValidator
            .Setup(x => x.ValidateAsync(patchedDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _repository
            .Setup(x => x.UpdateAsync(workout, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new PatchSetCommandHandler(
            _repository.Object,
            _mergePatchHelper.Object,
            _commandValidator.Object,
            _patchValidator.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(12, result.Value.Reps);
        Assert.Equal(55, result.Value.Weight);
        Assert.Equal(12, workout.Exercises[0].Sets[0].Reps);
        Assert.Equal(55, workout.Exercises[0].Sets[0].Weight);
        _repository.Verify(x => x.UpdateAsync(workout, It.IsAny<CancellationToken>()), Times.Once);
    }

    private static Workout CreateWorkout(Guid userId, Guid workoutId)
    {
        return new Workout
        {
            Id = workoutId.ToString(),
            UserId = userId.ToString(),
            Title = "Leg Day",
            Type = WorkoutType.Strength,
            Duration = TimeSpan.FromMinutes(45),
            CaloriesBurned = 400,
            WorkoutDate = new DateTime(2026, 3, 26),
            CreatedAt = new DateTime(2026, 3, 1),
            Exercises =
            [
                new Exercise
                {
                    Name = "Squat",
                    Sets = [ new Set { Reps = 10, Weight = 50 } ]
                }
            ]
        };
    }
}

