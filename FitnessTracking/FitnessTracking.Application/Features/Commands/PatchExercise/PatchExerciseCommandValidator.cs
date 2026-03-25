using FluentValidation;

namespace FitnessTracking.Application.Features.Commands.PatchExercise;

public class PatchExerciseCommandValidator : AbstractValidator<PatchExerciseCommand>
{
    public PatchExerciseCommandValidator()
    {
        RuleFor(x => x.WorkoutId).NotEmpty();
        RuleFor(x => x.ExerciseName).NotEmpty();
        RuleFor(x => x.Patch).NotNull();
    }
}

