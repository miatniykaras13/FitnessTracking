using Carter;
using FitnessTracking.Api.Extensions;
using FitnessTracking.Application.Abstractions;
using FitnessTracking.Application.Requests;
using FitnessTracking.Shared.Contracts;

namespace FitnessTracking.Api.Endpoints.Put;

public class UpdateSet : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPut("/workouts/{workoutId:guid}/exercises/{exerciseName}/sets/{setIndex:int}", async (
            Guid workoutId,
            string exerciseName,
            int setIndex,
            UpdateSetDto dto,
            HttpContext httpContext,
            IWorkoutsService workoutsService,
            CancellationToken ct = default) =>
        {
            var request = new UpdateSetRequest(workoutId, exerciseName, setIndex, dto);
            var result = await workoutsService.UpdateSetAsync(request, ct);
            return result.ToHttpResult(httpContext);
        })
        .WithTags("Sets")
        .WithName("UpdateSet")
        .WithSummary("Update set")
        .WithDescription("Replaces a single set by index for the specified exercise.")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithOpenApi();
}

