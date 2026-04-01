using FitnessTracking.Application.Constants;
using FitnessTracking.Shared.Contracts;
using FluentValidation;

namespace FitnessTracking.Application.Validators;

public class MergePatchSetDtoValidator : AbstractValidator<MergePatchSetDto>
{
    public MergePatchSetDtoValidator()
    {
        When(x => x.Reps is not null, () =>
        {
            RuleFor(x => x.Reps)
                .NotNull()
                .GreaterThan(ValidationConstants.MinZeroBasedIndex);
        });
        When(x => x.Weight is not null, () =>
        {
            RuleFor(x => x.Weight)
                .NotNull()
                .GreaterThanOrEqualTo(ValidationConstants.MinZeroBasedIndex);
        });
    }
}
