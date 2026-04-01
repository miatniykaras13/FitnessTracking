using CSharpFunctionalExtensions;
using FitnessTracking.Api.Constants;
using FitnessTracking.Api.Exceptions;
using FitnessTracking.Shared.Errors;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracking.Api.Extensions;

public static class ResultExtensions
{
    public static Microsoft.AspNetCore.Http.IResult ToHttpResult<T>(
        this Result<T, List<Error>> result,
        HttpContext httpContext,
        Func<T, Microsoft.AspNetCore.Http.IResult>? onSuccess = null)
    {
        if (result.IsSuccess)
        {
            return onSuccess is null ? Results.Ok(result.Value) : onSuccess(result.Value);
        }

        var problems = result.Error.Count == 1
            ? ToProblem(result.Error[0], httpContext)
            : ToProblem(result.Error, httpContext);
        return Results.Problem(problems);
    }

    public static Microsoft.AspNetCore.Http.IResult ToHttpResult(
        this UnitResult<List<Error>> result,
        HttpContext httpContext,
        Func<Microsoft.AspNetCore.Http.IResult>? onSuccess = null)
    {
        if (result.IsSuccess)
        {
            return onSuccess is null ? Results.NoContent() : onSuccess();
        }

        var problem = result.Error.Count == 1
            ? ToProblem(result.Error[0], httpContext)
            : ToProblem(result.Error, httpContext);
        return Results.Problem(problem);
    }

    private static ProblemDetails ToProblem(Error error, HttpContext httpContext)
    {
        var statusCode = error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };

        var problem = new ProblemDetails()
        {
            Title = error.GetName(),
            Status = statusCode,
            Detail = error.Message,
            Instance = httpContext.Request.Path,
            Extensions =
            {
                [ApiConstants.ProblemDetails.ErrorCodeExtensionKey] = error.Code,
                [ApiConstants.ProblemDetails.TraceIdExtensionKey] = httpContext.TraceIdentifier
            }
        };

        return problem;
    }


    private static ProblemDetails ToProblem(List<Error> errors, HttpContext httpContext)
    {
        if (errors.Count == 0)
        {
            throw new InvalidErrorListStateException("Cannot convert an empty list of errors to ProblemDetails.");
        }

        var problem = ToProblem(errors[0], httpContext);

        problem.Detail = ApiConstants.ProblemDetails.MultipleErrorsDetail;
        var otherProblems = errors.Select(e => ToProblem(e, httpContext)).ToList();

        problem.Extensions.Add(ApiConstants.ProblemDetails.OtherProblemsExtensionKey, otherProblems);

        return problem;
    }
}