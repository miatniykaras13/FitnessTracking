using FitnessTracking.Application.Constants;
using FitnessTracking.Application.Pagination;
using FluentValidation;

namespace FitnessTracking.Application.Validators;

public class PageParametersValidator : AbstractValidator<PageParameters>
{
    public PageParametersValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(ValidationConstants.DefaultPageNumber);
        RuleFor(x => x.PageSize)
            .InclusiveBetween(ValidationConstants.MinPositiveNumber, ValidationConstants.MaxPageSize);
    }
}

