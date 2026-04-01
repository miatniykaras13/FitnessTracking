using FitnessTracking.Application.Abstractions.Auth;
using FitnessTracking.Application.Features.Commands.LoginUser;
using FitnessTracking.Domain.Models;
using Moq;

namespace FitnessTracking.Application.Tests.Features.Commands;

public class LoginUserCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Return_Unauthorized_When_User_Not_Found()
    {
        var tokenService = new Mock<ITokenService>();
        var authService = new Mock<IAuthService>();
        var command = new LoginUserCommand("user@example.com", "Pass123$");

        authService
            .Setup(x => x.ValidateCredentialsAsync(command.Email, command.Password, It.IsAny<CancellationToken>()))
            .ReturnsAsync((AuthUser?)null);

        var handler = new LoginUserCommandHandler(tokenService.Object, authService.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Error, x => x.Code == "user.is_unauthorized");
    }

    [Fact]
    public async Task Handle_Should_Return_Token_When_Credentials_Are_Valid()
    {
        var tokenService = new Mock<ITokenService>();
        var authService = new Mock<IAuthService>();
        var command = new LoginUserCommand("user@example.com", "Pass123$");
        var user = new AuthUser(Guid.NewGuid().ToString(), command.Email);

        authService
            .Setup(x => x.ValidateCredentialsAsync(command.Email, command.Password, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        tokenService.Setup(x => x.GenerateToken(user)).Returns("token");

        var handler = new LoginUserCommandHandler(tokenService.Object, authService.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("token", result.Value.AccessToken);
    }
}
