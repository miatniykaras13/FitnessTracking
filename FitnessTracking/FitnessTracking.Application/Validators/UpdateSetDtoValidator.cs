using FitnessTracking.Application.Constants;
using FitnessTracking.Shared.Contracts;
using FluentValidation;

namespace FitnessTracking.Application.Validators;

public class UpdateSetDtoValidator : AbstractValidator<UpdateSetDto>
{
    public UpdateSetDtoValidator()
    {
        RuleFor(x => x.Reps).GreaterThan(ValidationConstants.MinZeroBasedIndex);
        RuleFor(x => x.Weight).GreaterThanOrEqualTo(ValidationConstants.MinZeroBasedIndex);
    }
}

