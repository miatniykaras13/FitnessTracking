using FitnessTracking.Application.Pagination;
using FitnessTracking.Application.Validators;
using FluentValidation.TestHelper;

namespace FitnessTracking.Application.Tests.Validators;

public class PageParametersValidatorTests
{
    private readonly PageParametersValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_PageNumber_Less_Than_1()
    {
        var model = new PageParameters(0, 10);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.PageNumber);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public void Should_Have_Error_When_PageSize_Out_Of_Range(int pageSize)
    {
        var model = new PageParameters(1, pageSize);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public void Should_Not_Have_Errors_When_PageParameters_Are_Valid()
    {
        var model = new PageParameters(2, 25);

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveAnyValidationErrors();
    }
}

