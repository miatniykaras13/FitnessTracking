using System.Text.Json;
using System.Text.Json.Nodes;
using Carter;
using FitnessTracking.Api.Extensions;
using FitnessTracking.Application.Features.Commands.PatchExercise;
using FitnessTracking.Shared.Contracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracking.Api.Endpoints.Patch;

public class PatchExercise : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPatch("/workouts/{workoutId:guid}/exercises/{exerciseName}", async (
            Guid workoutId,
            string exerciseName,
            HttpRequest httpRequest,
            HttpContext httpContext,
            ISender sender,
            CancellationToken ct = default) =>
        {
            var patchObject =
                await JsonSerializer.DeserializeAsync<JsonObject>(
                    httpRequest.Body,
                    new JsonSerializerOptions(JsonSerializerDefaults.Web),
                    ct);

            if (patchObject is null || patchObject.GetType() != typeof(JsonObject))
                throw new InvalidOperationException("Patch body must be a JsonObject");
            
            var command = new PatchExerciseCommand(workoutId, exerciseName, patchObject);
            var result = await sender.Send(command, ct);
            return result.ToHttpResult(httpContext);
        })
        .WithTags("Exercises")
        .WithName("PatchExercise")
        .WithSummary("Partially update exercise")
        .WithDescription("Applies a JSON Merge Patch document to an exercise.")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .WithMetadata(new ConsumesAttribute(typeof(MergePatchExerciseDto), "application/merge-patch+json"))
        .WithOpenApi();
}