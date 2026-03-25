using Carter;
using FitnessTracking.Api.Extensions;
using FitnessTracking.Application.Features.Commands.DeleteSet;
using MediatR;

namespace FitnessTracking.Api.Endpoints.Delete;

public class DeleteSet : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapDelete("/workouts/{workoutId:guid}/exercises/{exerciseName}/sets/{setIndex:int}", async (
            Guid workoutId,
            string exerciseName,
            int setIndex,
            HttpContext httpContext,
            ISender sender,
            CancellationToken ct = default) =>
        {
            var command = new DeleteSetCommand(workoutId, exerciseName, setIndex);
            var result = await sender.Send(command, ct);
            return result.ToHttpResult(httpContext);
        })
        .WithTags("Sets")
        .WithName("DeleteSet")
        .WithSummary("Delete set")
        .WithDescription("Deletes a set by index from the specified exercise.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithOpenApi();
}

