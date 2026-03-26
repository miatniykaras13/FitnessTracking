using FluentValidation;

namespace FitnessTracking.Application.Features.Commands.DeleteWorkoutPhoto;

public class DeleteWorkoutPhotoCommandValidator : AbstractValidator<DeleteWorkoutPhotoCommand>
{
    public DeleteWorkoutPhotoCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.WorkoutId).NotEmpty();
        RuleFor(x => x.PhotoId).NotEmpty();
    }
}

