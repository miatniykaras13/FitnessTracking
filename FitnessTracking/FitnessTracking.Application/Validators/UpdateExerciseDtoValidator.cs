using FitnessTracking.Shared.Contracts;
using FluentValidation;

namespace FitnessTracking.Application.Validators;

public class UpdateExerciseDtoValidator : AbstractValidator<UpdateExerciseDto>
{
    public UpdateExerciseDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleForEach(x => x.Sets).SetValidator(new SetDtoValidator());
    }
}

