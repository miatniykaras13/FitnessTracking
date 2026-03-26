using FitnessTracking.Application.Extensions;
using FluentValidation.Results;

namespace FitnessTracking.Application.Tests.Extensions;

public class FluentValidationExtensionsTests
{
    [Fact]
    public void ToError_Should_Convert_Path_To_SnakeCase()
    {
        var failure = new ValidationFailure("WorkoutDate", "must not be empty");

        var error = failure.ToError("workout");

        Assert.Equal("workout.workout_date.is_invalid", error.Code);
        Assert.Equal("must not be empty", error.Message);
    }

    [Fact]
    public void ToErrors_Should_Convert_All_Failures()
    {
        var failures = new List<ValidationFailure>
        {
            new("PageParameters.PageSize", "invalid"),
            new("Title", "required")
        };

        var errors = failures.ToErrors("query");

        Assert.Equal(2, errors.Count);
        Assert.Equal("query.page_parameters.page_size.is_invalid", errors[0].Code);
        Assert.Equal("query.title.is_invalid", errors[1].Code);
    }
}


