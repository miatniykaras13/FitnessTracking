using Carter;
using FitnessTracking.Api.Extensions;
using FitnessTracking.Application.Abstractions;
using FitnessTracking.Application.Requests;

namespace FitnessTracking.Api.Endpoints.Delete;

public class DeleteSet : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapDelete("/workouts/{workoutId:guid}/exercises/{exerciseName}/sets/{setIndex:int}", async (
            Guid workoutId,
            string exerciseName,
            int setIndex,
            HttpContext httpContext,
            IWorkoutsService workoutsService,
            CancellationToken ct = default) =>
        {
            var request = new DeleteSetRequest(workoutId, exerciseName, setIndex);
            var result = await workoutsService.DeleteSetAsync(request, ct);
            return result.ToHttpResult(httpContext);
        })
        .WithTags("Sets")
        .WithName("DeleteSet")
        .WithSummary("Delete set")
        .WithDescription("Deletes a set by index from the specified exercise.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithOpenApi();
}

