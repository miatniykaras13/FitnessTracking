using Carter;
using FitnessTracking.Api.Extensions;
using FitnessTracking.Application.Features.Commands.AddExercise;
using FitnessTracking.Shared.Contracts;
using MediatR;

namespace FitnessTracking.Api.Endpoints.Post;

public class AddExerciseToWorkout : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPost("/workouts/{workoutId:guid}/exercises", async (
            Guid workoutId,
            AddExerciseDto dto,
            HttpContext httpContext,
            ISender sender,
            CancellationToken ct = default) =>
        {
            var command = new AddExerciseCommand(workoutId, dto);
            var result = await sender.Send(command, ct);

            return result.ToHttpResult(httpContext, created =>
                Results.Created($"/workouts/{workoutId}/exercises/{Uri.EscapeDataString(created.Name)}", created));
        })
        .WithTags("Exercises")
        .WithName("AddExerciseToWorkout")
        .WithSummary("Add exercise to workout")
        .WithDescription("Adds a new exercise with sets to the specified workout.")
        .Produces(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .WithOpenApi();
}

