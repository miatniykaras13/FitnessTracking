using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.Auth;
using FitnessTracking.Application.Features.Commands.RegisterUser;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Errors;
using Moq;

namespace FitnessTracking.Application.Tests.Features.Commands;

public class RegisterUserCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Create_User_When_Data_Is_Valid()
    {
        var authService = new Mock<IAuthService>();
        var command = new RegisterUserCommand("newuser@example.com", "Pass123$");

        authService
            .Setup(x => x.RegisterAsync(command.Email, command.Password, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<AuthUser, List<Error>>(new AuthUser(Guid.NewGuid().ToString(), command.Email)));

        var handler = new RegisterUserCommandHandler(authService.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.False(string.IsNullOrWhiteSpace(result.Value.UserId));
    }

    [Fact]
    public async Task Handle_Should_Return_Failure_When_Registration_Returns_Errors()
    {
        var authService = new Mock<IAuthService>();
        var command = new RegisterUserCommand("newuser@example.com", "Pass123$");
        var errors = new List<Error> { Error.Conflict("user.email", "Email already exists") };

        authService
            .Setup(x => x.RegisterAsync(command.Email, command.Password, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<AuthUser, List<Error>>(errors));

        var handler = new RegisterUserCommandHandler(authService.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.NotEmpty(result.Error);
    }
}
