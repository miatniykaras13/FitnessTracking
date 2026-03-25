using FitnessTracking.Shared.Contracts;
using FluentValidation;

namespace FitnessTracking.Application.Validators;

public class AddSetDtoValidator : AbstractValidator<AddSetDto>
{
    public AddSetDtoValidator()
    {
        RuleFor(x => x.Reps).GreaterThan(0);
        RuleFor(x => x.Weight).GreaterThanOrEqualTo(0);
    }
}

