using FluentValidation;

namespace FitnessTracking.Application.Features.Queries.GetExercisesByWorkoutId;

public class GetExercisesByWorkoutIdQueryValidator : AbstractValidator<GetExercisesByWorkoutIdQuery>
{
    public GetExercisesByWorkoutIdQueryValidator()
    {
        RuleFor(x => x.WorkoutId).NotEmpty();
    }
}

