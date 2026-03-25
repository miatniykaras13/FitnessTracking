using FitnessTracking.Shared.Contracts;
using FluentValidation;

namespace FitnessTracking.Application.Validators;

public class SetDtoValidator : AbstractValidator<SetDto>
{
    public SetDtoValidator()
    {
        RuleFor(x => x.Reps).GreaterThan(0);
        RuleFor(x => x.Weight).GreaterThanOrEqualTo(0);
    }
}

