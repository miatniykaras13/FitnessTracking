using FitnessTracking.Domain.Enums;
using FitnessTracking.Shared.Contracts;
using FluentValidation;

namespace FitnessTracking.Application.Validators;

public class CreateWorkoutDtoValidator : AbstractValidator<CreateWorkoutDto>
{
    public CreateWorkoutDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Type)
            .NotEmpty()
            .Must(BeValidWorkoutType);
        RuleFor(x => x.Duration).GreaterThan(TimeSpan.Zero);
        RuleFor(x => x.CaloriesBurned).GreaterThanOrEqualTo(0);
        RuleFor(x => x.WorkoutDate).NotEqual(default(DateTime));
    }

    private static bool BeValidWorkoutType(string type) =>
        Enum.TryParse<WorkoutType>(type, true, out _);
}