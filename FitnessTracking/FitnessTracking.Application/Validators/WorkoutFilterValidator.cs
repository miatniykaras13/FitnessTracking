using FitnessTracking.Application.Filters;
using FitnessTracking.Domain.Enums;
using FluentValidation;

namespace FitnessTracking.Application.Validators;

public class WorkoutFilterValidator : AbstractValidator<WorkoutFilter>
{
    public WorkoutFilterValidator()
    {
        RuleFor(x => x.Type)
            .Must(type => string.IsNullOrWhiteSpace(type) || Enum.TryParse<WorkoutType>(type, true, out _));

        RuleFor(x => x.WorkoutDateFrom)
            .LessThanOrEqualTo(x => x.WorkoutDateTo)
            .When(x => x.WorkoutDateFrom.HasValue && x.WorkoutDateTo.HasValue);

        RuleFor(x => x.DurationFrom)
            .GreaterThan(TimeSpan.Zero)
            .When(x => x.DurationFrom.HasValue);

        RuleFor(x => x.DurationTo)
            .GreaterThan(TimeSpan.Zero)
            .When(x => x.DurationTo.HasValue);

        RuleFor(x => x.DurationFrom)
            .LessThanOrEqualTo(x => x.DurationTo)
            .When(x => x.DurationFrom.HasValue && x.DurationTo.HasValue);
    }
}

