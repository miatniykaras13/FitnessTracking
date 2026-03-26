using FitnessTracking.Domain.Enums;
using FitnessTracking.Shared.Contracts;
using FluentValidation;

namespace FitnessTracking.Application.Validators;

public class MergePatchWorkoutDtoValidator : AbstractValidator<MergePatchWorkoutDto>
{
    public MergePatchWorkoutDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Type)
            .NotEmpty()
            .Must(BeValidWorkoutType);
        RuleFor(x => x.Duration)
            .GreaterThan(TimeSpan.Zero);
        RuleFor(x => x.CaloriesBurned)
            .GreaterThanOrEqualTo(0);
        RuleFor(x => x.WorkoutDate)
            .NotEqual(default(DateTime));
    }

    private static bool BeValidWorkoutType(string? type) =>
        !string.IsNullOrWhiteSpace(type) && Enum.TryParse<WorkoutType>(type, true, out _);
}

