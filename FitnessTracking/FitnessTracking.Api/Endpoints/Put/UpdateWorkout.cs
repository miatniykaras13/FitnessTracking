using System.Security.Claims;
using Carter;
using FitnessTracking.Api.Extensions;
using FitnessTracking.Application.Features.Commands.UpdateWorkout;
using FitnessTracking.Shared.Contracts;
using MediatR;

namespace FitnessTracking.Api.Endpoints.Put;

public class UpdateWorkout : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPut("/workouts/{workoutId:guid}", async (
            Guid workoutId, 
            ClaimsPrincipal user,
            UpdateWorkoutDto dto,
            HttpContext httpContext,
            ISender sender,
            CancellationToken ct = default) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
                return Results.Unauthorized();

            var command = new UpdateWorkoutCommand(
                workoutId,
                Guid.Parse(userId),
                dto);
            
            var response = await sender.Send(command, ct);
            return response.ToHttpResult(httpContext);
        })
        .RequireAuthorization()
        .WithTags("Workouts")
        .WithName("UpdateWorkout")
        .WithSummary("Update workout")
        .WithDescription("Replaces workout fields with the provided payload.")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithOpenApi();
}


