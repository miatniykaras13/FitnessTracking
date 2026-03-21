using Carter;
using FitnessTracking.Application.Abstractions;
using FitnessTracking.Shared.Contracts.Requests;

namespace FitnessTracking.Api.Endpoints;

public class GetWorkoutById : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet("/workouts/{workoutId:guid}", async (
            Guid workoutId,
            IWorkoutsService workoutsService,
            CancellationToken ct = default) =>
        {
            var request = new GetWorkoutByIdRequest(workoutId);

            var workoutResponse = await workoutsService.GetByIdAsync(request, ct);

            if (workoutResponse is null)
            {
                return Results.NotFound();
            }
            return Results.Ok(workoutResponse);
        });
}