using System.Security.Claims;
using Carter;
using FitnessTracking.Api.Extensions;
using FitnessTracking.Application.Features.Commands.DeleteWorkoutPhoto;
using MediatR;

namespace FitnessTracking.Api.Endpoints.Delete;

public class DeleteWorkoutPhoto : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapDelete("/workouts/{workoutId:guid}/photos/{photoId:guid}", async (
            Guid workoutId,
            Guid photoId,
            ClaimsPrincipal user,
            HttpContext httpContext,
            ISender sender,
            CancellationToken ct = default) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
                return Results.Unauthorized();

            var command = new DeleteWorkoutPhotoCommand(Guid.Parse(userId), workoutId, photoId);
            var result = await sender.Send(command, ct);
            return result.ToHttpResult(httpContext);
        })
        .RequireAuthorization()
        .WithTags("Workouts")
        .WithName("DeleteWorkoutPhoto")
        .WithSummary("Delete workout photo")
        .WithDescription("Deletes workout photo metadata and local file.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithOpenApi();
}

