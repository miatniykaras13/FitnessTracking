using FluentValidation;

namespace FitnessTracking.Application.Features.Commands.AddPhotosToWorkout;

public class AddPhotosToWorkoutCommandValidator : AbstractValidator<AddPhotosToWorkoutCommand>
{
    private const int MaxFileSizeBytes = 10 * 1024 * 1024;

    public AddPhotosToWorkoutCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.WorkoutId).NotEmpty();
        RuleFor(x => x.FileName).NotEmpty();
        RuleFor(x => x.FileContent).NotNull().Must(content => content.Length > 0);
        RuleFor(x => x.FileContent.Length).LessThanOrEqualTo(MaxFileSizeBytes);
    }
}

