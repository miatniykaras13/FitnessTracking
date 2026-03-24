using System.Text.Json;
using System.Text.Json.Nodes;
using Carter;
using FitnessTracking.Api.Extensions;
using FitnessTracking.Application.Abstractions;
using FitnessTracking.Shared.Contracts.Requests;
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
            IWorkoutsService workoutsService,
            CancellationToken ct = default) =>
        {
            var patchObject =
                await JsonSerializer.DeserializeAsync<JsonObject>(
                    httpRequest.Body,
                    new JsonSerializerOptions(JsonSerializerDefaults.Web),
                    ct);

            if (patchObject is null || patchObject.GetType() != typeof(JsonObject))
                throw new InvalidOperationException("Patch body must be a JsonObject");
            
            var request = new PatchExerciseRequest(workoutId, exerciseName, patchObject);
            var result = await workoutsService.PatchExerciseAsync(request, ct);
            return result.ToHttpResult(httpContext);
        })
        .WithMetadata(new ConsumesAttribute("application/merge-patch+json"));
}