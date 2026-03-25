using System.Text.Json;
using System.Text.Json.Nodes;
using Carter;
using FitnessTracking.Api.Extensions;
using FitnessTracking.Application.Features.Commands.PatchSet;
using FitnessTracking.Shared.Contracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracking.Api.Endpoints.Patch;

public class PatchSet : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPatch("/workouts/{workoutId:guid}/exercises/{exerciseName}/sets/{setIndex:int}", async (
            Guid workoutId,
            string exerciseName,
            int setIndex,
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

            if (patchObject is null)
            {
                throw new InvalidOperationException("Patch body must be a JsonObject");
            }

            var command = new PatchSetCommand(workoutId, exerciseName, setIndex, patchObject);
            var result = await sender.Send(command, ct);
            return result.ToHttpResult(httpContext);
        })
        .WithTags("Sets")
        .WithName("PatchSet")
        .WithSummary("Partially update set")
        .WithDescription("Applies a JSON Merge Patch document to a set by index.")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithMetadata(new ConsumesAttribute(typeof(MergePatchSetDto), "application/merge-patch+json"))
        .WithOpenApi();
}