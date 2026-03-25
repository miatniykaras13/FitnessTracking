using FitnessTracking.Shared.Contracts;
using FluentValidation;

namespace FitnessTracking.Application.Validators;

public class AddExerciseDtoValidator : AbstractValidator<AddExerciseDto>
{
    public AddExerciseDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Sets).NotNull().NotEmpty();
        RuleForEach(x => x.Sets).SetValidator(new AddSetDtoValidator());
    }
}

