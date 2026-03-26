using FitnessTracking.Application.Validators;
using FluentValidation;

namespace FitnessTracking.Application.Features.Commands.AddSet;

public class AddSetCommandValidator : AbstractValidator<AddSetCommand>
{
    public AddSetCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.WorkoutId).NotEmpty();
        RuleFor(x => x.ExerciseName).NotEmpty();
        RuleFor(x => x.SetDto)
            .NotNull()
            .SetValidator(new AddSetDtoValidator());
    }
}

