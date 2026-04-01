using FitnessTracking.Application.Abstractions.Repositories;
using FitnessTracking.Application.Features.Commands.CreateWorkout;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Contracts;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace FitnessTracking.Application.Tests.Features.Commands;

public class CreateWorkoutCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Return_Failure_For_Invalid_Workout_Type()
    {
        var repository = new Mock<IWorkoutsRepository>();
        var validator = new Mock<IValidator<CreateWorkoutCommand>>();
        var command = new CreateWorkoutCommand(Guid.NewGuid(), new CreateWorkoutDto("Title", "WrongType", TimeSpan.FromMinutes(30), 200, DateTime.UtcNow));

        validator.Setup(x => x.ValidateAsync(It.IsAny<CreateWorkoutCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var handler = new CreateWorkoutCommandHandler(repository.Object, validator.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Error, x => x.Code == "workout.type.is_invalid");
    }

    [Fact]
    public async Task Handle_Should_Create_Workout_When_Request_Is_Valid()
    {
        var repository = new Mock<IWorkoutsRepository>();
        var validator = new Mock<IValidator<CreateWorkoutCommand>>();
        var userId = Guid.NewGuid();
        var command = new CreateWorkoutCommand(userId, new CreateWorkoutDto("Cardio", "Cardio", TimeSpan.FromMinutes(30), 250, new DateTime(2026, 3, 26)));

        validator.Setup(x => x.ValidateAsync(It.IsAny<CreateWorkoutCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        repository.Setup(x => x.AddAsync(It.IsAny<Workout>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Workout w, CancellationToken _) => w);

        var handler = new CreateWorkoutCommandHandler(repository.Object, validator.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Cardio", result.Value.Title);
        Assert.Equal(userId, result.Value.UserId);
        repository.Verify(x => x.AddAsync(It.IsAny<Workout>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}

