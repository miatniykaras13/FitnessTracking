using FitnessTracking.Application.Validators;
using FluentValidation;

namespace FitnessTracking.Application.Features.Queries.GetWorkoutsByUserId;

public class GetWorkoutsByUserIdQueryValidator : AbstractValidator<GetWorkoutsByUserIdQuery>
{
    public GetWorkoutsByUserIdQueryValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Filter)
            .NotNull()
            .SetValidator(new WorkoutFilterValidator());
        RuleFor(x => x.SortParameters)
            .NotNull()
            .SetValidator(new SortParametersValidator());
        RuleFor(x => x.PageParameters)
            .NotNull()
            .SetValidator(new PageParametersValidator());
    }
}

