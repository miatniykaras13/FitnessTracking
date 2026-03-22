using Carter;
using FitnessTracking.Application.Abstractions;
using FitnessTracking.Shared.Contracts.Requests;

namespace FitnessTracking.Api.Endpoints;

public class UpdateWorkout : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPut("/workouts/{workoutId:guid}", async (
            Guid workoutId,
            UpdateWorkoutRequest request,
            IWorkoutsService workoutsService,
            CancellationToken ct = default) =>
        {
            var response = await workoutsService.UpdateAsync(workoutId, request, ct);
            return Results.Ok(response);
        });
}


