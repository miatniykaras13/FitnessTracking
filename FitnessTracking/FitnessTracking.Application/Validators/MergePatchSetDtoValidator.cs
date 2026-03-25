using FitnessTracking.Shared.Contracts;
using FluentValidation;

namespace FitnessTracking.Application.Validators;

public class MergePatchSetDtoValidator : AbstractValidator<MergePatchSetDto>
{
    public MergePatchSetDtoValidator()
    {
        RuleFor(x => x.Reps)
            .NotNull()
            .GreaterThan(0);
        RuleFor(x => x.Weight)
            .NotNull()
            .GreaterThanOrEqualTo(0);
    }
}

