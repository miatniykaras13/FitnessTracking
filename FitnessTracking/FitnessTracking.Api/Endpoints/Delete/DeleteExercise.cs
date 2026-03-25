using Carter;
using FitnessTracking.Api.Extensions;
using FitnessTracking.Application.Features.Commands.DeleteExercise;
using MediatR;

namespace FitnessTracking.Api.Endpoints.Delete;

public class DeleteExercise : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapDelete("/workouts/{workoutId:guid}/exercises/{exerciseName}", async (
            Guid workoutId,
            string exerciseName,
            HttpContext httpContext,
            ISender sender,
            CancellationToken ct = default) =>
        {
            var command = new DeleteExerciseCommand(workoutId, exerciseName);
            var result = await sender.Send(command, ct);
            return result.ToHttpResult(httpContext);
        })
        .WithTags("Exercises")
        .WithName("DeleteExercise")
        .WithSummary("Delete exercise")
        .WithDescription("Deletes an exercise from the specified workout.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithOpenApi();
}

