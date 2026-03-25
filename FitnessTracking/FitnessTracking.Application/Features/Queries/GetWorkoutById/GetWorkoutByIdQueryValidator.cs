using FluentValidation;

namespace FitnessTracking.Application.Features.Queries.GetWorkoutById;

public class GetWorkoutByIdQueryValidator : AbstractValidator<GetWorkoutByIdQuery>
{
    public GetWorkoutByIdQueryValidator()
    {
        RuleFor(x => x.WorkoutId).NotEmpty();
    }
}

