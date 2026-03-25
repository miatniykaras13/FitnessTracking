using System.Text.RegularExpressions;
using FitnessTracking.Shared.Errors;
using FluentValidation.Results;

namespace FitnessTracking.Application.Extensions;

public static class FluentValidationExtensions
{
    public static Error ToError(this ValidationFailure failure, string obj)
    {
        var path = string.Join(
            ".",
            failure.PropertyName
                .Split('.')
                .Select(ToSnakeCase));

        return Error.Validation($"{obj}.{path}", failure.ErrorMessage);
    }


    public static List<Error> ToErrors(this List<ValidationFailure> failures, string obj) =>
        failures.Select(x => x.ToError(obj)).ToList();

    private static string ToSnakeCase(string s) => Regex.Replace(s, "([a-z])([A-Z])", "$1_$2").ToLower();
}