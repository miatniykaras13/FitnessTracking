using Carter;
using FitnessTracking.Api.Extensions;
using FitnessTracking.Application.Abstractions;
using FitnessTracking.Shared.Contracts.Dtos;
using FitnessTracking.Shared.Contracts.Requests;

namespace FitnessTracking.Api.Endpoints.Put;

public class UpdateExercise : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPut("/workouts/{workoutId:guid}/exercises/{exerciseName}", async (
            Guid workoutId,
            string exerciseName,
            UpdateExerciseDto dto,
            HttpContext httpContext,
            IWorkoutsService workoutsService,
            CancellationToken ct = default) =>
        {
            var request = new UpdateExerciseRequest(workoutId, exerciseName, dto);
            var result = await workoutsService.UpdateExerciseAsync(request, ct);
            return result.ToHttpResult(httpContext);
        })
        .WithTags("Exercises")
        .WithName("UpdateExercise")
        .WithSummary("Update exercise")
        .WithDescription("Replaces exercise name and sets for the specified exercise.")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .WithOpenApi();
}

