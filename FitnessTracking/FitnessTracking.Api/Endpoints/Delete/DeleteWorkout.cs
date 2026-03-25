using Carter;
using FitnessTracking.Api.Extensions;
using FitnessTracking.Application.Features.Commands.DeleteWorkout;
using MediatR;

namespace FitnessTracking.Api.Endpoints.Delete;

public class DeleteWorkout : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapDelete("/workouts/{workoutId:guid}", async (
            Guid workoutId,
            HttpContext httpContext,
            ISender sender,
            CancellationToken ct = default) =>
        {
            var command = new DeleteWorkoutCommand(
                workoutId,
                Guid.NewGuid()); // todo: брать из claims principal
            var result = await sender.Send(command, ct);
            return result.ToHttpResult(httpContext);
        })
        .WithTags("Workouts")
        .WithName("DeleteWorkout")
        .WithSummary("Delete workout")
        .WithDescription("Deletes a workout by id.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithOpenApi();
}

