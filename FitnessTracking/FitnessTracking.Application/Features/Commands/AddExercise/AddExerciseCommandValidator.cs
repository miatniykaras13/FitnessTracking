using FitnessTracking.Application.Validators;
using FluentValidation;

namespace FitnessTracking.Application.Features.Commands.AddExercise;

public class AddExerciseCommandValidator : AbstractValidator<AddExerciseCommand>
{
    public AddExerciseCommandValidator()
    {
        RuleFor(x => x.WorkoutId).NotEmpty();
        RuleFor(x => x.ExerciseDto)
            .NotNull()
            .SetValidator(new AddExerciseDtoValidator());
    }
}

