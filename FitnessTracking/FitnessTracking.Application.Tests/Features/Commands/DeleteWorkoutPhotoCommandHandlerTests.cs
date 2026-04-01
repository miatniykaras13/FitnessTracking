using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.Helpers;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Features.Commands.DeleteWorkoutPhoto;
using FitnessTracking.Domain.Enums;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Errors;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace FitnessTracking.Application.Tests.Features.Commands;

public class DeleteWorkoutPhotoCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Return_NotFound_When_Photo_Belongs_To_Another_Workout()
    {
        var photosRepository = new Mock<IWorkoutPhotosRepository>();
        var workoutsRepository = new Mock<IWorkoutsRepository>();
        var fileStorage = new Mock<ILocalFileStorage>();
        var validator = new Mock<IValidator<DeleteWorkoutPhotoCommand>>();

        var userId = Guid.NewGuid();
        var workoutId = Guid.NewGuid();
        var anotherWorkoutId = Guid.NewGuid();
        var photoId = Guid.NewGuid();
        var command = new DeleteWorkoutPhotoCommand(userId, workoutId, photoId);
        var workout = CreateWorkout(userId, workoutId, photoId);
        var photo = new WorkoutPhoto
        {
            Id = photoId.ToString(),
            WorkoutId = anotherWorkoutId.ToString(),
            Path = "uploads/workouts/photo.jpg",
            CreatedAt = new DateTime(2026, 3, 26)
        };

        validator.Setup(x => x.ValidateAsync(It.IsAny<DeleteWorkoutPhotoCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        workoutsRepository.Setup(x => x.GetByIdAsync(workoutId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(workout);
        photosRepository.Setup(x => x.GetByIdAsync(photoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(photo);

        var handler = new DeleteWorkoutPhotoCommandHandler(photosRepository.Object, workoutsRepository.Object, fileStorage.Object, validator.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Error, x => x.Code == "workout.photo.not_found");
        photosRepository.Verify(x => x.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Delete_Photo_When_Request_Is_Valid()
    {
        var photosRepository = new Mock<IWorkoutPhotosRepository>();
        var workoutsRepository = new Mock<IWorkoutsRepository>();
        var fileStorage = new Mock<ILocalFileStorage>();
        var validator = new Mock<IValidator<DeleteWorkoutPhotoCommand>>();

        var userId = Guid.NewGuid();
        var workoutId = Guid.NewGuid();
        var photoId = Guid.NewGuid();
        var command = new DeleteWorkoutPhotoCommand(userId, workoutId, photoId);
        var workout = CreateWorkout(userId, workoutId, photoId);
        var photo = new WorkoutPhoto
        {
            Id = photoId.ToString(),
            WorkoutId = workoutId.ToString(),
            Path = "uploads/workouts/photo.jpg",
            CreatedAt = new DateTime(2026, 3, 26)
        };

        validator.Setup(x => x.ValidateAsync(It.IsAny<DeleteWorkoutPhotoCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        workoutsRepository.Setup(x => x.GetByIdAsync(workoutId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(workout);

        photosRepository.Setup(x => x.GetByIdAsync(photoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(photo);
        photosRepository.Setup(x => x.DeleteAsync(photoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        fileStorage.Setup(x => x.DeleteWorkoutPhotoAsync(photo.Path, It.IsAny<CancellationToken>()))
            .ReturnsAsync(UnitResult.Success<Error>());

        var handler = new DeleteWorkoutPhotoCommandHandler(photosRepository.Object, workoutsRepository.Object, fileStorage.Object, validator.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    private static Workout CreateWorkout(Guid userId, Guid workoutId, Guid photoId) => new()
    {
        Id = workoutId.ToString(),
        UserId = userId.ToString(),
        Title = "Workout",
        Type = WorkoutType.Strength,
        Duration = TimeSpan.FromMinutes(40),
        CaloriesBurned = 300,
        WorkoutDate = new DateTime(2026, 3, 26),
        CreatedAt = new DateTime(2026, 3, 1),
        ProgressPhotos = [new WorkoutPhoto { Id = photoId.ToString(), WorkoutId = workoutId.ToString(), Path = "uploads/workouts/photo.jpg", CreatedAt = new DateTime(2026, 3, 20) }]
    };
}

