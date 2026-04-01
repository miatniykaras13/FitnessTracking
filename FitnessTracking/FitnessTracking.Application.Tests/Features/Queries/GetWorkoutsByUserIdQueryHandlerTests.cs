using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Features.Queries.GetWorkoutsByUserId;
using FitnessTracking.Application.Filters;
using FitnessTracking.Application.Pagination;
using FitnessTracking.Application.Sorting;
using FitnessTracking.Domain.Enums;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Errors;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace FitnessTracking.Application.Tests.Features.Queries;

public class GetWorkoutsByUserIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_Should_Return_Workouts_And_Total_When_Data_Exists()
    {
        var repository = new Mock<IWorkoutsRepository>();
        var validator = new Mock<IValidator<GetWorkoutsByUserIdQuery>>();
        var userId = Guid.NewGuid();
        var filter = new WorkoutFilter(null, null, null, null, null);
        var sort = new SortParameters("CreatedAt", SortDirection.Descending);
        var page = new PageParameters(1, 10);
        var query = new GetWorkoutsByUserIdQuery(userId, filter, sort, page);
        var workouts = new List<Workout> { CreateWorkout(Guid.NewGuid(), userId) };

        validator.Setup(x => x.ValidateAsync(It.IsAny<GetWorkoutsByUserIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        repository.Setup(x => x.GetByUserIdAsync(userId, filter, sort, page, It.IsAny<CancellationToken>()))
            .ReturnsAsync(workouts);
        repository.Setup(x => x.GetCountByUserIdWithFilterAsync(userId, filter, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new GetWorkoutsByUserIdQueryHandler(repository.Object, validator.Object);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value.Total);
        Assert.Single(result.Value.Workouts);
    }

    private static Workout CreateWorkout(Guid workoutId, Guid userId) => new()
    {
        Id = workoutId.ToString(),
        UserId = userId.ToString(),
        Title = "Workout",
        Type = WorkoutType.Strength,
        Duration = TimeSpan.FromMinutes(45),
        CaloriesBurned = 350,
        WorkoutDate = new DateTime(2026, 3, 26),
        CreatedAt = new DateTime(2026, 3, 1)
    };
}

