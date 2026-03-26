using FitnessTracking.Application.Validators;
using FluentValidation;

namespace FitnessTracking.Application.Features.Commands.UpdateExercise;

public class UpdateExerciseCommandValidator : AbstractValidator<UpdateExerciseCommand>
{
    public UpdateExerciseCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.WorkoutId).NotEmpty();
        RuleFor(x => x.ExerciseName).NotEmpty();
        RuleFor(x => x.ExerciseDto)
            .NotNull()
            .SetValidator(new UpdateExerciseDtoValidator());
    }
}

