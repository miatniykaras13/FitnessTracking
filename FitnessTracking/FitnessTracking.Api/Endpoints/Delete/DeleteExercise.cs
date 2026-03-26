using System.Security.Claims;
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
            ClaimsPrincipal user,
            HttpContext httpContext,
            ISender sender,
            CancellationToken ct = default) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
                return Results.Unauthorized();

            var command = new DeleteExerciseCommand(Guid.Parse(userId), workoutId, exerciseName);
            var result = await sender.Send(command, ct);
            return result.ToHttpResult(httpContext);
        })
        .RequireAuthorization()
        .WithTags("Exercises")
        .WithName("DeleteExercise")
        .WithSummary("Delete exercise")
        .WithDescription("Deletes an exercise from the specified workout.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithOpenApi();
}

