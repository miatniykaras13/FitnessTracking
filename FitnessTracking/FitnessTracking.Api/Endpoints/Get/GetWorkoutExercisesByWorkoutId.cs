using Carter;
using FitnessTracking.Api.Extensions;
using FitnessTracking.Application.Abstractions;
using FitnessTracking.Shared.Contracts.Requests;

namespace FitnessTracking.Api.Endpoints.Get;

public class GetWorkoutExercisesByWorkoutId : ICarterModule
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
        });
}


