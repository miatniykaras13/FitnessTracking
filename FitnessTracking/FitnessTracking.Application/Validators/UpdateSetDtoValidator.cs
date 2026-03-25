using FitnessTracking.Shared.Contracts;
using FluentValidation;

namespace FitnessTracking.Application.Validators;

public class UpdateSetDtoValidator : AbstractValidator<UpdateSetDto>
{
    public UpdateSetDtoValidator()
    {
        RuleFor(x => x.Reps).GreaterThan(0);
        RuleFor(x => x.Weight).GreaterThanOrEqualTo(0);
    }
}

