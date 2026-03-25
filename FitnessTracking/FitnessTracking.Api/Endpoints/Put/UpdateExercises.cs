using Carter;
using FitnessTracking.Api.Extensions;
using FitnessTracking.Application.Abstractions;
using FitnessTracking.Application.Requests;
using FitnessTracking.Shared.Contracts;

namespace FitnessTracking.Api.Endpoints.Put;

public class UpdateExercises : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPut("/workouts/{workoutId:guid}/exercises", async (
            Guid workoutId,
            UpdateExercisesDto dto,
            HttpContext httpContext,
            IWorkoutsService workoutsService,
            CancellationToken ct = default) =>
        {
            var request = new UpdateExercisesRequest(workoutId, dto);
            var result = await workoutsService.UpdateExercisesAsync(request, ct);
            return result.ToHttpResult(httpContext);
        })
        .WithTags("Exercises")
        .WithName("UpdateExercises")
        .WithSummary("Replace workout exercises")
        .WithDescription("Replaces the full exercise list for the specified workout.")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .WithOpenApi();
}


