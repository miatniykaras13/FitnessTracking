using FluentValidation;

namespace FitnessTracking.Application.Features.Commands.AddPhotosToWorkout;

public class AddPhotosToWorkoutCommandValidator : AbstractValidator<AddPhotosToWorkoutCommand>
{
    public AddPhotosToWorkoutCommandValidator()
    {
        RuleFor(x => x.WorkoutId).NotEmpty();
    }
}

