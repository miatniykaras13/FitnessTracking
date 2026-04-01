using FitnessTracking.Application.Constants;
using FluentValidation;

namespace FitnessTracking.Application.Features.Commands.RegisterUser;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(ValidationConstants.MinPasswordLength);
    }
}

