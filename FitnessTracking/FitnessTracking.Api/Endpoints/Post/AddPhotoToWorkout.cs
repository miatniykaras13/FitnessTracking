using Carter;
using FitnessTracking.Api.Extensions;
using FitnessTracking.Application.Features.Commands.AddPhotosToWorkout;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracking.Api.Endpoints.Post;

public class AddPhotoToWorkout : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPost("/workouts/{workoutId:guid}/photos", async (
            Guid workoutId,
            [FromForm] AddPhotoToWorkoutForm form,
            HttpContext httpContext,
            ISender sender,
            CancellationToken ct = default) =>
        {
            var file = form.File;

            await using var stream = file.OpenReadStream();
            await using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream, ct);

            var command = new AddPhotosToWorkoutCommand(
                workoutId,
                file.FileName,
                memoryStream.ToArray());

            var result = await sender.Send(command, ct);
            return result.ToHttpResult(httpContext, created =>
                Results.Created($"/workouts/{workoutId}/photos/{created.PhotoId}", created));
        })
        .DisableAntiforgery()
        .WithTags("Workouts")
        .WithName("AddPhotoToWorkout")
        .WithSummary("Add photo to workout")
        .WithDescription("Uploads and attaches a single progress photo to the specified workout.")
        .Accepts<AddPhotoToWorkoutForm>("multipart/form-data")
        .Produces(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithOpenApi();
}

public class AddPhotoToWorkoutForm
{
    public IFormFile File { get; set; } = default!;
}


