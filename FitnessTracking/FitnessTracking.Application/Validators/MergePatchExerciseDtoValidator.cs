using FitnessTracking.Shared.Contracts;
using FluentValidation;

namespace FitnessTracking.Application.Validators;

public class MergePatchExerciseDtoValidator : AbstractValidator<MergePatchExerciseDto>
{
    public MergePatchExerciseDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Sets).NotNull().NotEmpty();
        RuleForEach(x => x.Sets).SetValidator(new AddSetDtoValidator());
    }
}

