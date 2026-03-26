using Carter;
using FitnessTracking.Api.Extensions;
using FitnessTracking.Application.Features.Queries.GetWorkoutById;
using MediatR;

namespace FitnessTracking.Api.Endpoints.Get;

public class GetWorkoutById : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet("/workouts/{workoutId:guid}", async (
            Guid workoutId,
            HttpContext httpContext,
            ISender sender,
            CancellationToken ct = default) =>
        {
            var query = new GetWorkoutByIdQuery(workoutId);
            var workoutResponse = await sender.Send(query, ct);
            return workoutResponse.ToHttpResult(httpContext);
        })
        .WithTags("Workouts")
        .WithName("GetWorkoutById")
        .WithSummary("Get workout by id")
        .WithDescription("Returns a single workout with all basic workout fields.")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .AllowAnonymous()
        .WithOpenApi();
}