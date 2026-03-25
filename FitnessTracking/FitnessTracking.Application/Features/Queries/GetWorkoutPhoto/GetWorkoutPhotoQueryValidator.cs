using FluentValidation;

namespace FitnessTracking.Application.Features.Queries.GetWorkoutPhoto;

public class GetWorkoutPhotoQueryValidator : AbstractValidator<GetWorkoutPhotoQuery>
{
    public GetWorkoutPhotoQueryValidator()
    {
        RuleFor(x => x.WorkoutId).NotEmpty();
        RuleFor(x => x.PhotoId).NotEmpty();
    }
}

