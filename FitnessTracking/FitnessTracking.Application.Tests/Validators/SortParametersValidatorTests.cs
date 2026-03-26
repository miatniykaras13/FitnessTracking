using FitnessTracking.Application.Sorting;
using FitnessTracking.Application.Validators;
using FluentValidation.TestHelper;

namespace FitnessTracking.Application.Tests.Validators;

public class SortParametersValidatorTests
{
    private readonly SortParametersValidator _validator = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("WorkoutDate")]
    [InlineData("caloriesburned")]
    [InlineData("CreatedAt")]
    public void Should_Not_Have_Error_For_Allowed_OrderBy(string? orderBy)
    {
        var model = new SortParameters(orderBy, SortDirection.Descending);

        var result = _validator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.OrderBy);
    }

    [Fact]
    public void Should_Have_Error_For_Not_Allowed_OrderBy()
    {
        var model = new SortParameters("Duration", SortDirection.Ascending);

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.OrderBy);
    }
}

