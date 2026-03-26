using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.Helpers;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Features.Commands.AddPhotosToWorkout;
using FitnessTracking.Domain.Enums;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Errors;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace FitnessTracking.Application.Tests.Features.Commands;

public class AddPhotosToWorkoutCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Add_Photo_When_Request_Is_Valid()
    {
        var fileStorage = new Mock<ILocalFileStorage>();
        var photosRepository = new Mock<IWorkoutPhotosRepository>();
        var workoutsRepository = new Mock<IWorkoutsRepository>();
        var validator = new Mock<IValidator<AddPhotosToWorkoutCommand>>();

        var userId = Guid.NewGuid();
        var workoutId = Guid.NewGuid();
        var command = new AddPhotosToWorkoutCommand(userId, workoutId, "photo.jpg", [1, 2, 3]);
        var workout = CreateWorkout(userId, workoutId);

        validator.Setup(x => x.ValidateAsync(It.IsAny<AddPhotosToWorkoutCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        workoutsRepository.Setup(x => x.GetByIdAsync(workoutId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Workout, Error>(workout));
        fileStorage.Setup(x => x.SaveWorkoutPhotoAsync(workoutId, "photo.jpg", command.FileContent, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<string, Error>("uploads/workouts/photo.jpg"));
        photosRepository.Setup(x => x.AddAsync(It.IsAny<WorkoutPhoto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(UnitResult.Success<Error>());
        workoutsRepository.Setup(x => x.UpdateAsync(workout, It.IsAny<CancellationToken>()))
            .ReturnsAsync(UnitResult.Success<Error>());

        var handler = new AddPhotosToWorkoutCommandHandler(fileStorage.Object, photosRepository.Object, workoutsRepository.Object, validator.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("uploads/workouts/photo.jpg", result.Value.Path);
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

