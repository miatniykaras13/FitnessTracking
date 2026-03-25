using FluentValidation;

namespace FitnessTracking.Application.Features.Commands.PatchSet;

public class PatchSetCommandValidator : AbstractValidator<PatchSetCommand>
{
    public PatchSetCommandValidator()
    {
        RuleFor(x => x.WorkoutId).NotEmpty();
        RuleFor(x => x.ExerciseName).NotEmpty();
        RuleFor(x => x.SetIndex).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Patch).NotNull();
    }
}

