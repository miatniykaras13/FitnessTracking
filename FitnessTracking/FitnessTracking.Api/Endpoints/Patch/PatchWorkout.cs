using System.Text.Json;
using System.Text.Json.Nodes;
using Carter;
using FitnessTracking.Api.Extensions;
using FitnessTracking.Application.Abstractions;
using FitnessTracking.Shared.Contracts.Dtos;
using FitnessTracking.Shared.Contracts.Requests;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracking.Api.Endpoints.Patch;

public class PatchWorkout : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPatch("/workouts/{workoutId:guid}", async (
            Guid workoutId,
            HttpRequest httpRequest,
            HttpContext httpContext,
            IWorkoutsService workoutsService,
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

            var request = new PatchWorkoutRequest(workoutId, patchObject);
            var result = await workoutsService.PatchAsync(request, ct);
            return result.ToHttpResult(httpContext);
        })
        .WithTags("Workouts")
        .WithName("PatchWorkout")
        .WithSummary("Partially update workout")
        .WithDescription("Applies a JSON Merge Patch document to the workout.")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithMetadata(new ConsumesAttribute(typeof(MergePatchWorkoutDto), "application/merge-patch+json"))
        .WithOpenApi();
}