using FitnessTracking.Application.Features.Commands.LoginUser;
using FluentValidation.TestHelper;

namespace FitnessTracking.Application.Tests.Validators;

public class LoginUserCommandValidatorTests
{
    private readonly LoginUserCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_Email_Is_Invalid()
    {
        var command = new LoginUserCommand("invalid-email", "qwerty123");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_Have_Error_When_Password_Is_Empty()
    {
        var command = new LoginUserCommand("user@example.com", "");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Should_Not_Have_Errors_When_Command_Is_Valid()
    {
        var command = new LoginUserCommand("user@example.com", "qwerty123");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}

