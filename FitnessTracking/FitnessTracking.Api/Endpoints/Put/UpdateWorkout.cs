using Carter;
using FitnessTracking.Api.Extensions;
using FitnessTracking.Application.Abstractions;
using FitnessTracking.Application.Requests;
using FitnessTracking.Shared.Contracts;

namespace FitnessTracking.Api.Endpoints.Put;

public class UpdateWorkout : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPut("/workouts/{workoutId:guid}", async (
            Guid workoutId, 
            UpdateWorkoutDto dto,
            HttpContext httpContext,
            IWorkoutsService workoutsService,
            CancellationToken ct = default) =>
        {
            var request = new UpdateWorkoutRequest(
                workoutId,
                Guid.NewGuid(), // todo: брать из claims principal
                dto);
            
            var response = await workoutsService.UpdateAsync(request, ct);
            return response.ToHttpResult(httpContext);
        })
        .WithTags("Workouts")
        .WithName("UpdateWorkout")
        .WithSummary("Update workout")
        .WithDescription("Replaces workout fields with the provided payload.")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithOpenApi();
}


