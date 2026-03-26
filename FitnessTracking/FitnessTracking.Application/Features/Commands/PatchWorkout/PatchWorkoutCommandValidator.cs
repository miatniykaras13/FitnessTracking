using FluentValidation;

namespace FitnessTracking.Application.Features.Commands.PatchWorkout;

public class PatchWorkoutCommandValidator : AbstractValidator<PatchWorkoutCommand>
{
    public PatchWorkoutCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.WorkoutId).NotEmpty();
        RuleFor(x => x.Patch).NotNull();
    }
}

