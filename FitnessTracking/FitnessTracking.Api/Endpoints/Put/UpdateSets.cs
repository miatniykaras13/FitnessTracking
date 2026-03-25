using Carter;
using FitnessTracking.Api.Extensions;
using FitnessTracking.Application.Features.Commands.UpdateSets;
using FitnessTracking.Shared.Contracts;
using MediatR;

namespace FitnessTracking.Api.Endpoints.Put;

public class UpdateSets : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPut("/workouts/{workoutId:guid}/exercises/{exerciseName}/sets", async (
            Guid workoutId,
            string exerciseName,
            UpdateSetsDto dto,
            HttpContext httpContext,
            ISender sender,
            CancellationToken ct = default) =>
        {
            var command = new UpdateSetsCommand(workoutId, exerciseName, dto);
            var result = await sender.Send(command, ct);
            return result.ToHttpResult(httpContext);
        })
        .WithTags("Sets")
        .WithName("UpdateSets")
        .WithSummary("Replace exercise sets")
        .WithDescription("Replaces the full set list for the specified exercise.")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithOpenApi();
}


