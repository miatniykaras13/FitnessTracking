using Carter;
using FitnessTracking.Application.Abstractions;
using FitnessTracking.Shared.Contracts.Requests;

namespace FitnessTracking.Api.Endpoints;

public class DeleteWorkout : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapDelete("/workouts/{workoutId:guid}", async (
            Guid workoutId,
            IWorkoutsService workoutsService,
            CancellationToken ct = default) =>
        {
            var request = new DeleteWorkoutRequest(workoutId);
            await workoutsService.DeleteAsync(request, ct);
            return Results.NoContent();
        });
}

