using FitnessTracking.Application.Validators;
using FluentValidation;

namespace FitnessTracking.Application.Features.Commands.UpdateSets;

public class UpdateSetsCommandValidator : AbstractValidator<UpdateSetsCommand>
{
    public UpdateSetsCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.WorkoutId).NotEmpty();
        RuleFor(x => x.ExerciseName).NotEmpty();
        RuleFor(x => x.SetDtos)
            .NotNull()
            .SetValidator(new UpdateSetsDtoValidator());
    }
}

