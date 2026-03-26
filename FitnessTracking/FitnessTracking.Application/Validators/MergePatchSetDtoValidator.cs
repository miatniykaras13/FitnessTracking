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
                .GreaterThan(0);
        });
        When(x => x.Weight is not null, () =>
        {
            RuleFor(x => x.Weight)
                .NotNull()
                .GreaterThanOrEqualTo(0);
        });
    }
}