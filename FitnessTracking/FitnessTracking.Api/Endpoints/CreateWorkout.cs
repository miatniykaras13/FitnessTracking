using Carter;
using FitnessTracking.Application.Abstractions;
using FitnessTracking.Shared.Contracts.Requests;

namespace FitnessTracking.Api.Endpoints;

public class CreateWorkout : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPost("/workouts", async (
            CreateWorkoutRequest request,
            IWorkoutsService workoutsService,
            CancellationToken ct = default) =>
        {
            var response = await workoutsService.AddAsync(Guid.NewGuid(), request, ct); //todo: брать user id из claims principal
            return Results.Created($"/workouts/{response.WorkoutId}", response);
        });
}