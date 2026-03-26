using System.Security.Claims;
using Carter;
using FitnessTracking.Api.Extensions;
using FitnessTracking.Application.Features.Commands.UpdateExercise;
using FitnessTracking.Shared.Contracts;
using MediatR;

namespace FitnessTracking.Api.Endpoints.Put;

public class UpdateExercise : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPut("/workouts/{workoutId:guid}/exercises/{exerciseName}", async (
            Guid workoutId,
            string exerciseName,
            ClaimsPrincipal user,
            UpdateExerciseDto dto,
            HttpContext httpContext,
            ISender sender,
            CancellationToken ct = default) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
                return Results.Unauthorized();

            var command = new UpdateExerciseCommand(Guid.Parse(userId), workoutId, exerciseName, dto);
            var result = await sender.Send(command, ct);
            return result.ToHttpResult(httpContext);
        })
        .RequireAuthorization()
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

