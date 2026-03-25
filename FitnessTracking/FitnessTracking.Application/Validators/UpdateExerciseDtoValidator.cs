using FitnessTracking.Shared.Contracts;
using FluentValidation;

namespace FitnessTracking.Application.Validators;

public class UpdateExerciseDtoValidator : AbstractValidator<UpdateExerciseDto>
{
    public UpdateExerciseDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Sets).NotNull().NotEmpty();
        RuleForEach(x => x.Sets).SetValidator(new SetDtoValidator());
    }
}

