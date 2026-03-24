using Carter;
using FitnessTracking.Api.Extensions;
using FitnessTracking.Application.Abstractions;
using FitnessTracking.Shared.Contracts.Dtos;
using FitnessTracking.Shared.Contracts.Requests;

namespace FitnessTracking.Api.Endpoints.Post;

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
                dto);
            var response = await workoutsService.AddAsync(request, ct);
            return response.ToHttpResult(httpContext, created => Results.Created($"/workouts/{created.WorkoutId}", created));
        })
        .WithTags("Workouts")
        .WithName("CreateWorkout")
        .WithSummary("Create workout")
        .WithDescription("Creates a new workout for the current user.")
        .Produces(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithOpenApi();
}