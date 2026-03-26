using FitnessTracking.Application.Abstractions.Auth;
using FitnessTracking.Application.Features.Commands.LoginUser;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace FitnessTracking.Application.Tests.Features.Commands;

public class LoginUserCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Return_Unauthorized_When_User_Not_Found()
    {
        var tokenService = new Mock<ITokenService>();
        var userManager = CreateUserManagerMock();
        var command = new LoginUserCommand("user@example.com", "Pass123$");

        userManager.Setup(x => x.FindByEmailAsync(command.Email))
            .ReturnsAsync((IdentityUser?)null);

        var handler = new LoginUserCommandHandler(tokenService.Object, userManager.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Error, x => x.Code == "user.is_unauthorized");
    }

    [Fact]
    public async Task Handle_Should_Return_Token_When_Credentials_Are_Valid()
    {
        var tokenService = new Mock<ITokenService>();
        var userManager = CreateUserManagerMock();
        var command = new LoginUserCommand("user@example.com", "Pass123$");
        var user = new IdentityUser { Id = Guid.NewGuid().ToString(), Email = command.Email, UserName = command.Email };

        userManager.Setup(x => x.FindByEmailAsync(command.Email)).ReturnsAsync(user);
        userManager.Setup(x => x.CheckPasswordAsync(user, command.Password)).ReturnsAsync(true);
        tokenService.Setup(x => x.GenerateToken(user)).Returns("token");

        var handler = new LoginUserCommandHandler(tokenService.Object, userManager.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("token", result.Value.AccessToken);
    }

    private static Mock<UserManager<IdentityUser>> CreateUserManagerMock()
    {
        var store = new Mock<IUserStore<IdentityUser>>();
        return new Mock<UserManager<IdentityUser>>(
            store.Object,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!);
    }
}

