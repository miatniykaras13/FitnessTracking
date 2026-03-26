using System.Security.Claims;
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
            ClaimsPrincipal user,
            HttpContext httpContext,
            ISender sender,
            CancellationToken ct = default) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
                return Results.Unauthorized();

            var command = new DeleteSetCommand(Guid.Parse(userId), workoutId, exerciseName, setIndex);
            var result = await sender.Send(command, ct);
            return result.ToHttpResult(httpContext);
        })
        .RequireAuthorization()
        .WithTags("Sets")
        .WithName("DeleteSet")
        .WithSummary("Delete set")
        .WithDescription("Deletes a set by index from the specified exercise.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithOpenApi();
}

