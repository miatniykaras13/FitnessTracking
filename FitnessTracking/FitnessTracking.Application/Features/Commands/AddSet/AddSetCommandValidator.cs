using FitnessTracking.Application.Validators;
using FluentValidation;

namespace FitnessTracking.Application.Features.Commands.AddSet;

public class AddSetCommandValidator : AbstractValidator<AddSetCommand>
{
    public AddSetCommandValidator()
    {
        RuleFor(x => x.WorkoutId).NotEmpty();
        RuleFor(x => x.ExerciseName).NotEmpty();
        RuleFor(x => x.SetDto)
            .NotNull()
            .SetValidator(new AddSetDtoValidator());
    }
}

