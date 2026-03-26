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

        workoutsRepository.SetupSequence(x => x.GetByIdAsync(workoutId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Workout, Error>(workout))
            .ReturnsAsync(Result.Success<Workout, Error>(workout));

        photosRepository.Setup(x => x.GetByWorkoutIdAndPhotoIdAsync(workoutId, photoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<WorkoutPhoto, Error>(photo));
        photosRepository.Setup(x => x.DeleteByWorkoutIdAndPhotoIdAsync(workoutId, photoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(UnitResult.Success<Error>());
        workoutsRepository.Setup(x => x.UpdateAsync(workout, It.IsAny<CancellationToken>()))
            .ReturnsAsync(UnitResult.Success<Error>());
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

