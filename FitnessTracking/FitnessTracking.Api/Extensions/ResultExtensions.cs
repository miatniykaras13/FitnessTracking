using CSharpFunctionalExtensions;
using FitnessTracking.Shared.Errors;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracking.Api.Extensions;

public static class ResultExtensions
{
    public static Microsoft.AspNetCore.Http.IResult ToHttpResult<T>(
        this Result<T, Error> result,
        HttpContext httpContext,
        Func<T, Microsoft.AspNetCore.Http.IResult>? onSuccess = null)
    {
        if (result.IsSuccess)
        {
            return onSuccess is null ? Results.Ok(result.Value) : onSuccess(result.Value);
        }

        var problem = ToProblem(result.Error, httpContext);

        return Results.Problem(problem);
    }

    public static Microsoft.AspNetCore.Http.IResult ToHttpResult<T>(
        this Result<T, List<Error>> result,
        HttpContext httpContext,
        Func<T, Microsoft.AspNetCore.Http.IResult>? onSuccess = null)
    {
        if (result.IsSuccess)
        {
            return onSuccess is null ? Results.Ok(result.Value) : onSuccess(result.Value);
        }

        var problems = ToProblem(result.Error, httpContext);
        return Results.Problem(problems);
    }

    public static Microsoft.AspNetCore.Http.IResult ToHttpResult(
        this UnitResult<Error> result,
        HttpContext httpContext,
        Func<Microsoft.AspNetCore.Http.IResult>? onSuccess = null)
    {
        if (result.IsSuccess)
        {
            return onSuccess is null ? Results.NoContent() : onSuccess();
        }

        var problem = ToProblem(result.Error, httpContext);
        return Results.Problem(problem);
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

        var problem = ToProblem(result.Error, httpContext);
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
                ["errorCode"] = error.Code,
                ["traceId"] = httpContext.TraceIdentifier
            }
        };

        return problem;
    }


    private static ProblemDetails ToProblem(List<Error> errors, HttpContext httpContext)
    {
        if (errors.Count == 0)
            throw new InvalidOperationException("Cannot convert an empty list of errors to a ProblemDetails object.");

        var problem = ToProblem(errors[0], httpContext);

        problem.Detail = "Multiple errors occurred. See the 'otherProblems' extension for details.";
        var otherProblems = errors.Select(e => ToProblem(e, httpContext)).ToList();

        problem.Extensions.Add("otherProblems", otherProblems);
        
        return problem;
    }
}