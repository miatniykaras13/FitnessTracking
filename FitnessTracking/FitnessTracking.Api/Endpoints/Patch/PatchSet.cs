using System.Text.Json;
using System.Text.Json.Nodes;
using System.Security.Claims;
using Carter;
using FitnessTracking.Api.Exceptions;
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
                throw new InvalidPatchDocumentException("Patch body must be a JsonObject.");
            }

            var command = new PatchSetCommand(Guid.Parse(userId), workoutId, exerciseName, setIndex, patchObject);
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
        .RequireAuthorization()
        .WithMetadata(new ConsumesAttribute(typeof(MergePatchSetDto), "application/merge-patch+json"))
        .WithOpenApi();
}