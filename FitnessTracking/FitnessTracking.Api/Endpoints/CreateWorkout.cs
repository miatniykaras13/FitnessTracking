using Carter;
using FitnessTracking.Application.Abstractions;
using FitnessTracking.Api.Extensions;
using FitnessTracking.Shared.Contracts.Dtos;
using FitnessTracking.Shared.Contracts.Requests;

namespace FitnessTracking.Api.Endpoints;

public class CreateWorkout : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPost("/workouts", async (
            CreateWorkoutDto dto,
            HttpContext httpContext,
            IWorkoutsService workoutsService,
            CancellationToken ct = default) =>
        {
            var request = new CreateWorkoutRequest(
                Guid.NewGuid(), // todo: брать из claims principal
                dto.Title,
                dto.Type,
                dto.Duration,
                dto.CaloriesBurned,
                dto.WorkoutDate);
            var response = await workoutsService.AddAsync(request, ct);
            return response.ToHttpResult(httpContext, created => Results.Created($"/workouts/{created.WorkoutId}", created));
        });
}