using Carter;
using FitnessTracking.Application.Abstractions;
using FitnessTracking.Shared.Contracts.Requests;

namespace FitnessTracking.Api.Endpoints;

public class GetWorkoutExercisesByWorkoutId : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet("/workouts/{workoutId:guid}/exercises", async (
            Guid workoutId,
            IWorkoutsService workoutsService,
            CancellationToken ct = default) =>
        {
            var request = new GetWorkoutByIdRequest(workoutId);
            var response = await workoutsService.GetExercisesByWorkoutIdAsync(request, ct);
            return Results.Ok(response);
        });
}


