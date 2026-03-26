using FitnessTracking.Application.Sorting;
using FluentValidation;

namespace FitnessTracking.Application.Validators;

public class SortParametersValidator : AbstractValidator<SortParameters>
{
    private static readonly string[] AllowedOrderBy =
    [
        "CaloriesBurned",
        "WorkoutDate",
        "CreatedAt"
    ];

    public SortParametersValidator()
    {
        RuleFor(x => x.OrderBy)
            .Must(orderBy => string.IsNullOrWhiteSpace(orderBy) ||
                             AllowedOrderBy.Any(a => a.Equals(orderBy, StringComparison.OrdinalIgnoreCase)));
    }
}

