using Carter;
using FitnessTracking.Application.Abstractions;
using FitnessTracking.Shared.Contracts.Requests;

namespace FitnessTracking.Api.Endpoints;

public class UpdateWorkout : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPut("/workouts/{workoutId:guid}", async (
            Guid workoutId,
            UpdateWorkoutRequest body,
            IWorkoutsService workoutsService,
            CancellationToken ct = default) =>
        {
            var request = body with { WorkoutId = workoutId };

            var response = await workoutsService.UpdateAsync(request, ct);
            return Results.Ok(response);
        });
}


