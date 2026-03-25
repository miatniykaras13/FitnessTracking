using Carter;
using FitnessTracking.Api.Extensions;
using FitnessTracking.Application.Abstractions;
using FitnessTracking.Application.Requests;

namespace FitnessTracking.Api.Endpoints.Get;

public class GetExercisesByWorkoutId : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet("/workouts/{workoutId:guid}/exercises", async (
            Guid workoutId,
            HttpContext httpContext,
            IWorkoutsService workoutsService,
            CancellationToken ct = default) =>
        {
            var request = new GetExercisesByWorkoutIdRequest(workoutId);
            var response = await workoutsService.GetExercisesByWorkoutIdAsync(request, ct);
            return response.ToHttpResult(httpContext);
        })
        .WithTags("Exercises")
        .WithName("GetExercisesByWorkoutId")
        .WithSummary("Get workout exercises with sets by workout id")
        .WithDescription("Returns exercises for a workout including their sets.")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithOpenApi();
}


