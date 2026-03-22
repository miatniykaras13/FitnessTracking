using CSharpFunctionalExtensions;
using FitnessTracking.Shared.Errors;

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

        return ToProblem(result.Error, httpContext);
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

        return ToProblem(result.Error, httpContext);
    }

    private static Microsoft.AspNetCore.Http.IResult ToProblem(Error error, HttpContext httpContext)
    {
        var statusCode = error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };

        return Results.Problem(
            detail: error.Message,
            statusCode: statusCode,
            title: "CustomError",
            instance: httpContext.Request.Path,
            extensions: new Dictionary<string, object?>
            {
                ["errorCode"] = error.Code,
                ["traceId"] = httpContext.TraceIdentifier
            });
    }
}


