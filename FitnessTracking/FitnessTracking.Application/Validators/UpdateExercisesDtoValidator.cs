using FitnessTracking.Shared.Contracts;
using FluentValidation;

namespace FitnessTracking.Application.Validators;

public class UpdateExercisesDtoValidator : AbstractValidator<UpdateExercisesDto>
{
    public UpdateExercisesDtoValidator()
    {
        RuleFor(x => x.Exercises).NotNull().NotEmpty();
        RuleForEach(x => x.Exercises).SetValidator(new UpdateExerciseDtoValidator());
    }
}

