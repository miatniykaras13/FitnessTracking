using Carter;
using FitnessTracking.Api.Extensions;
using FitnessTracking.Application.Features.Queries.GetExercisesByWorkoutId;
using MediatR;

namespace FitnessTracking.Api.Endpoints.Get;

public class GetExercisesByWorkoutId : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet("/workouts/{workoutId:guid}/exercises", async (
            Guid workoutId,
            HttpContext httpContext,
            ISender sender,
            CancellationToken ct = default) =>
        {
            var query = new GetExercisesByWorkoutIdQuery(workoutId);
            var response = await sender.Send(query, ct);
            return response.ToHttpResult(httpContext);
        })
        .WithTags("Exercises")
        .WithName("GetExercisesByWorkoutId")
        .WithSummary("Get workout exercises with sets by workout id")
        .WithDescription("Returns exercises for a workout including their sets.")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .AllowAnonymous()
        .WithOpenApi();
}


