using Carter;
using FitnessTracking.Application.Abstractions;
using FitnessTracking.Api.Extensions;
using FitnessTracking.Shared.Contracts.Requests;

namespace FitnessTracking.Api.Endpoints;

public class CreateWorkout : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPost("/workouts", async (
            CreateWorkoutRequest request,
            HttpContext httpContext,
            IWorkoutsService workoutsService,
            CancellationToken ct = default) =>
        {
            var response = await workoutsService.AddAsync(Guid.NewGuid(), request, ct);
            return response.ToHttpResult(httpContext, created => Results.Created($"/workouts/{created.WorkoutId}", created));
        });
}