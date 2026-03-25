using FluentValidation;

namespace FitnessTracking.Application.Features.Commands.DeleteWorkout;

public class DeleteWorkoutCommandValidator : AbstractValidator<DeleteWorkoutCommand>
{
    public DeleteWorkoutCommandValidator()
    {
        RuleFor(x => x.WorkoutId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}

