using FitnessTracking.Application.Constants;
using FitnessTracking.Domain.Enums;
using FitnessTracking.Shared.Contracts;
using FluentValidation;

namespace FitnessTracking.Application.Validators;

public class MergePatchWorkoutDtoValidator : AbstractValidator<MergePatchWorkoutDto>
{
    public MergePatchWorkoutDtoValidator()
    {
        When(x => x.Title is not null, () => { RuleFor(x => x.Title).NotEmpty(); });
        When(x => x.Type is not null, () =>
        {
            RuleFor(x => x.Type)
                .NotEmpty()
                .Must(BeValidWorkoutType);
        });
        When(x => x.Duration is not null, () =>
        {
            RuleFor(x => x.Duration)
                .GreaterThan(TimeSpan.Zero);
        });
        When(x => x.CaloriesBurned is not null, () =>
        {
            RuleFor(x => x.CaloriesBurned)
                .GreaterThanOrEqualTo(ValidationConstants.MinZeroBasedIndex);
        });
        When(x => x.WorkoutDate is not null, () =>
        {
            RuleFor(x => x.WorkoutDate)
                .NotEqual(default(DateTime));
        });
    }

    private static bool BeValidWorkoutType(string? type) =>
        !string.IsNullOrWhiteSpace(type) && Enum.TryParse<WorkoutType>(type, true, out _);
}