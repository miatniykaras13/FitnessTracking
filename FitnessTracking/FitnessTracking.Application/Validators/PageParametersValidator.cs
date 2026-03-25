using FitnessTracking.Application.Pagination;
using FluentValidation;

namespace FitnessTracking.Application.Validators;

public class PageParametersValidator : AbstractValidator<PageParameters>
{
    public PageParametersValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}

