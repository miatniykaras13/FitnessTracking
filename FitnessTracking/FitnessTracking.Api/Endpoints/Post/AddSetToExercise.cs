using Carter;
using FitnessTracking.Api.Extensions;
using FitnessTracking.Application.Abstractions;
using FitnessTracking.Application.Requests;
using FitnessTracking.Shared.Contracts;

namespace FitnessTracking.Api.Endpoints.Post;

public class AddSetToExercise : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPost("/workouts/{workoutId:guid}/exercises/{exerciseName}/sets", async (
            Guid workoutId,
            string exerciseName,
            AddSetDto dto,
            HttpContext httpContext,
            IWorkoutsService workoutsService,
            CancellationToken ct = default) =>
        {
            var request = new AddSetRequest(workoutId, exerciseName, dto);
            var result = await workoutsService.AddSetAsync(request, ct);
            return result.ToHttpResult(httpContext, created =>
                Results.Created($"/workouts/{workoutId}/exercises/{Uri.EscapeDataString(exerciseName)}/sets", created));
        })
        .WithTags("Sets")
        .WithName("AddSetToExercise")
        .WithSummary("Add set to exercise")
        .WithDescription("Adds a new set to the specified exercise in a workout.")
        .Produces(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithOpenApi();
}

