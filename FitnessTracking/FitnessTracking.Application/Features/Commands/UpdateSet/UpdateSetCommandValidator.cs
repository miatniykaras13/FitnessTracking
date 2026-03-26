using FitnessTracking.Application.Validators;
using FluentValidation;

namespace FitnessTracking.Application.Features.Commands.UpdateSet;

public class UpdateSetCommandValidator : AbstractValidator<UpdateSetCommand>
{
    public UpdateSetCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.WorkoutId).NotEmpty();
        RuleFor(x => x.ExerciseName).NotEmpty();
        RuleFor(x => x.SetIndex).GreaterThanOrEqualTo(0);
        RuleFor(x => x.SetDto)
            .NotNull()
            .SetValidator(new UpdateSetDtoValidator());
    }
}

