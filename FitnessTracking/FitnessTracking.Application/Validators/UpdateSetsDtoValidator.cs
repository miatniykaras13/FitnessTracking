using FitnessTracking.Shared.Contracts;
using FluentValidation;

namespace FitnessTracking.Application.Validators;

public class UpdateSetsDtoValidator : AbstractValidator<UpdateSetsDto>
{
    public UpdateSetsDtoValidator()
    {
        RuleFor(x => x.Sets).NotNull().NotEmpty();
        RuleForEach(x => x.Sets).SetValidator(new UpdateSetDtoValidator());
    }
}

