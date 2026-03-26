using FitnessTracking.Application.Validators;
using FitnessTracking.Shared.Contracts;
using FluentValidation.TestHelper;

namespace FitnessTracking.Application.Tests.Validators;

public class MergePatchWorkoutDtoValidatorTests
{
    private readonly MergePatchWorkoutDtoValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_Type_Is_Invalid()
    {
        var dto = BuildValidDto();
        dto.Type = "Unknown";

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Type);
    }

    [Fact]
    public void Should_Have_Error_When_CaloriesBurned_Is_Negative()
    {
        var dto = BuildValidDto();
        dto.CaloriesBurned = -5;

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.CaloriesBurned);
    }

    [Fact]
    public void Should_Not_Have_Error_When_CaloriesBurned_Is_Zero()
    {
        var dto = BuildValidDto();
        dto.CaloriesBurned = 0;

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveValidationErrorFor(x => x.CaloriesBurned);
    }

    [Fact]
    public void Should_Have_Error_When_Duration_Is_Zero()
    {
        var dto = BuildValidDto();
        dto.Duration = TimeSpan.Zero;

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.Duration);
    }

    [Fact]
    public void Should_Have_Error_When_WorkoutDate_Is_Default()
    {
        var dto = BuildValidDto();
        dto.WorkoutDate = default(DateTime);

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.WorkoutDate);
    }

    [Fact]
    public void Should_Not_Have_Errors_When_Dto_Is_Valid()
    {
        var dto = BuildValidDto();

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveAnyValidationErrors();
    }

    private static MergePatchWorkoutDto BuildValidDto() => new()
    {
        Title = "Leg Day",
        Type = "Strength",
        Duration = TimeSpan.FromMinutes(45),
        CaloriesBurned = 300,
        WorkoutDate = DateTime.UtcNow.Date
    };
}


