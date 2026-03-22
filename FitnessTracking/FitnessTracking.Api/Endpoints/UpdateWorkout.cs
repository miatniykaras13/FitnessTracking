using Carter;
using FitnessTracking.Application.Abstractions;
using FitnessTracking.Api.Extensions;
using FitnessTracking.Shared.Contracts.Requests;

namespace FitnessTracking.Api.Endpoints;

public class UpdateWorkout : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPut("/workouts/{workoutId:guid}", async (
            Guid workoutId,
            UpdateWorkoutRequest request,
            HttpContext httpContext,
            IWorkoutsService workoutsService,
            CancellationToken ct = default) =>
        {
            var response = await workoutsService.UpdateAsync(workoutId, request, ct);
            return response.ToHttpResult(httpContext);
        });
}


