using FitnessTracking.Application.Validators;
using FluentValidation;

namespace FitnessTracking.Application.Features.Commands.CreateWorkout;

public class CreateWorkoutCommandValidator : AbstractValidator<CreateWorkoutCommand>
{
    public CreateWorkoutCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.WorkoutDto)
            .NotNull()
            .SetValidator(new CreateWorkoutDtoValidator());
    }
}