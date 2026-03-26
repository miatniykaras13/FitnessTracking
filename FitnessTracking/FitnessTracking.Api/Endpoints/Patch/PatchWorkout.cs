using System.Text.Json;
using System.Text.Json.Nodes;
using System.Security.Claims;
using Carter;
using FitnessTracking.Api.Extensions;
using FitnessTracking.Application.Features.Commands.PatchWorkout;
using FitnessTracking.Shared.Contracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracking.Api.Endpoints.Patch;

public class PatchWorkout : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPatch("/workouts/{workoutId:guid}", async (
            Guid workoutId,
            ClaimsPrincipal user,
            HttpRequest httpRequest,
            HttpContext httpContext,
            ISender sender,
            CancellationToken ct = default) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
                return Results.Unauthorized();

            var patchObject =
                await JsonSerializer.DeserializeAsync<JsonObject>(
                    httpRequest.Body,
                    new JsonSerializerOptions(JsonSerializerDefaults.Web),
                    ct);

            if (patchObject is null)
            {
                throw new InvalidOperationException("Patch body must be a JsonObject");
            }

            var command = new PatchWorkoutCommand(Guid.Parse(userId), workoutId, patchObject);
            var result = await sender.Send(command, ct);
            return result.ToHttpResult(httpContext);
        })
        .WithTags("Workouts")
        .WithName("PatchWorkout")
        .WithSummary("Partially update workout")
        .WithDescription("Applies a JSON Merge Patch document to the workout.")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization()
        .WithMetadata(new ConsumesAttribute(typeof(MergePatchWorkoutDto), "application/merge-patch+json"))
        .WithOpenApi();
}