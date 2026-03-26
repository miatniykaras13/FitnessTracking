using FitnessTracking.Application.Abstractions.Auth;
using FitnessTracking.Application.Features.Commands.RegisterUser;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace FitnessTracking.Application.Tests.Features.Commands;

public class RegisterCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Create_User_When_Data_Is_Valid()
    {
        var tokenService = new Mock<ITokenService>();
        var userManager = CreateUserManagerMock();
        var command = new RegisterUserCommand("newuser@example.com", "Pass123$");

        userManager
            .Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), command.Password))
            .ReturnsAsync(IdentityResult.Success);

        var handler = new RegisterCommandHandler(tokenService.Object, userManager.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.False(string.IsNullOrWhiteSpace(result.Value.UserId));
    }

    [Fact]
    public async Task Handle_Should_Return_Failure_When_Identity_Returns_Errors()
    {
        var tokenService = new Mock<ITokenService>();
        var userManager = CreateUserManagerMock();
        var command = new RegisterUserCommand("newuser@example.com", "Pass123$");

        userManager
            .Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), command.Password))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "DuplicateEmail", Description = "Email already exists" }));

        var handler = new RegisterCommandHandler(tokenService.Object, userManager.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.NotEmpty(result.Error);
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

