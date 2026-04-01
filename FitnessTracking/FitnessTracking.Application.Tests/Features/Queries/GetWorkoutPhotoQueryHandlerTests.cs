using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Features.Queries.GetWorkoutPhoto;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Errors;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace FitnessTracking.Application.Tests.Features.Queries;

public class GetWorkoutPhotoQueryHandlerTests
{
    private readonly Mock<IWorkoutsRepository> _workoutsRepository = new();
    private readonly Mock<IWorkoutPhotosRepository> _photosRepository = new();
    private readonly Mock<IValidator<GetWorkoutPhotoQuery>> _validator = new();

    [Fact]
    public async Task Handle_Should_Return_NotFound_When_Workout_Does_Not_Exist()
    {
        var workoutId = Guid.NewGuid();
        var photoId = Guid.NewGuid();
        var query = new GetWorkoutPhotoQuery(workoutId, photoId);

        _validator
            .Setup(x => x.ValidateAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _workoutsRepository
            .Setup(x => x.GetByIdAsync(workoutId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Workout?)null);

        var handler = new GetWorkoutPhotoQueryHandler(
            _workoutsRepository.Object,
            _photosRepository.Object,
            _validator.Object);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Error, x => x.Code == "workout.not_found");

        _photosRepository.Verify(
            x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Return_NotFound_When_Photo_Belongs_To_Another_Workout()
    {
        var workoutId = Guid.NewGuid();
        var anotherWorkoutId = Guid.NewGuid();
        var photoId = Guid.NewGuid();
        var query = new GetWorkoutPhotoQuery(workoutId, photoId);
        var workout = new Workout
        {
            Id = workoutId.ToString(),
            UserId = Guid.NewGuid().ToString(),
            Title = "Workout",
            Type = Domain.Enums.WorkoutType.Strength,
            Duration = TimeSpan.FromMinutes(40),
            CaloriesBurned = 300,
            WorkoutDate = new DateTime(2026, 3, 26),
            CreatedAt = new DateTime(2026, 3, 1)
        };
        var photo = new WorkoutPhoto
        {
            Id = photoId.ToString(),
            WorkoutId = anotherWorkoutId.ToString(),
            Path = "/uploads/workouts/photo.jpg",
            CreatedAt = new DateTime(2026, 3, 20)
        };

        _validator
            .Setup(x => x.ValidateAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _workoutsRepository
            .Setup(x => x.GetByIdAsync(workoutId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(workout);
        _photosRepository
            .Setup(x => x.GetByIdAsync(photoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(photo);

        var handler = new GetWorkoutPhotoQueryHandler(
            _workoutsRepository.Object,
            _photosRepository.Object,
            _validator.Object);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Error, x => x.Code == "workout.photo.not_found");
    }

    [Fact]
    public async Task Handle_Should_Return_Mapped_Response_When_Photo_Exists()
    {
        var workoutId = Guid.NewGuid();
        var photoId = Guid.NewGuid();
        var query = new GetWorkoutPhotoQuery(workoutId, photoId);
        var workout = new Workout
        {
            Id = workoutId.ToString(),
            UserId = Guid.NewGuid().ToString(),
            Title = "Workout",
            Type = Domain.Enums.WorkoutType.Strength,
            Duration = TimeSpan.FromMinutes(40),
            CaloriesBurned = 300,
            WorkoutDate = new DateTime(2026, 3, 26),
            CreatedAt = new DateTime(2026, 3, 1)
        };
        var photo = new WorkoutPhoto
        {
            Id = photoId.ToString(),
            WorkoutId = workoutId.ToString(),
            Path = "/uploads/workouts/photo.jpg",
            CreatedAt = new DateTime(2026, 3, 20)
        };

        _validator
            .Setup(x => x.ValidateAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _workoutsRepository
            .Setup(x => x.GetByIdAsync(workoutId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(workout);

        _photosRepository
            .Setup(x => x.GetByIdAsync(photoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(photo);

        var handler = new GetWorkoutPhotoQueryHandler(
            _workoutsRepository.Object,
            _photosRepository.Object,
            _validator.Object);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(photoId, result.Value.PhotoId);
        Assert.Equal(workoutId, result.Value.WorkoutId);
        Assert.Equal("/uploads/workouts/photo.jpg", result.Value.Path);
    }
}

