using FluentValidation;

namespace FitnessTracking.Application.Features.Commands.DeleteSet;

public class DeleteSetCommandValidator : AbstractValidator<DeleteSetCommand>
{
    public DeleteSetCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.WorkoutId).NotEmpty();
        RuleFor(x => x.ExerciseName).NotEmpty();
        RuleFor(x => x.SetIndex).GreaterThanOrEqualTo(0);
    }
}

