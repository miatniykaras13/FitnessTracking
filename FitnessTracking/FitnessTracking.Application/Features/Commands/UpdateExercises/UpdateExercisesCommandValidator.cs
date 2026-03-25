using FitnessTracking.Application.Validators;
using FluentValidation;

namespace FitnessTracking.Application.Features.Commands.UpdateExercises;

public class UpdateExercisesCommandValidator : AbstractValidator<UpdateExercisesCommand>
{
    public UpdateExercisesCommandValidator()
    {
        RuleFor(x => x.WorkoutId).NotEmpty();
        RuleFor(x => x.ExerciseDtos)
            .NotNull()
            .SetValidator(new UpdateExercisesDtoValidator());
    }
}

