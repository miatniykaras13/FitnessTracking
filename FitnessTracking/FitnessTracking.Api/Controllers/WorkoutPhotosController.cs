using FitnessTracking.Application.Features.Commands.AddPhotosToWorkout;
using FitnessTracking.Application.Features.Commands.DeleteWorkoutPhoto;
using FitnessTracking.Application.Features.Queries.GetWorkoutPhoto;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracking.Api.Controllers;

[ApiController]
public class WorkoutPhotosController(ISender sender) : ApiControllerBase(sender)
{
    [HttpPost("workouts/{workoutId:guid}/photos")]
    [Authorize]
    [Consumes("multipart/form-data")]
    [IgnoreAntiforgeryToken]
    [EndpointSummary("Upload workout photo")]
    [EndpointDescription("Uploads a photo and attaches it to the workout.")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Add(
        Guid workoutId,
        [FromForm] AddPhotoToWorkoutForm form,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized();
        }

        var file = form.File;
        var content = await ReadFileAsync(file, cancellationToken);

        var command = new AddPhotosToWorkoutCommand(
            userId,
            workoutId,
            file.FileName,
            content);

        return await Send(command, cancellationToken, created => Created($"/workouts/{workoutId}/photos/{created.PhotoId}", created));
    }

    [HttpGet("workouts/{workoutId:guid}/photos/{photoId:guid}")]
    [AllowAnonymous]
    [EndpointSummary("Get workout photo metadata")]
    [EndpointDescription("Returns workout photo metadata by identifier.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(Guid workoutId, Guid photoId, CancellationToken cancellationToken)
    {
        return await Send(new GetWorkoutPhotoQuery(workoutId, photoId), cancellationToken);
    }

    [HttpGet("workouts/{workoutId:guid}/photos/{photoId:guid}/content")]
    [AllowAnonymous]
    [EndpointSummary("Get workout photo content")]
    [EndpointDescription("Redirects to the static file URL for the workout photo.")]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetContent(Guid workoutId, Guid photoId, CancellationToken cancellationToken)
    {
        return await Send(new GetWorkoutPhotoQuery(workoutId, photoId), cancellationToken, success => Redirect(success.Path));
    }

    [HttpDelete("workouts/{workoutId:guid}/photos/{photoId:guid}")]
    [Authorize]
    [EndpointSummary("Delete workout photo")]
    [EndpointDescription("Deletes workout photo metadata and the associated file.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid workoutId, Guid photoId, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized();
        }

        return await Send(new DeleteWorkoutPhotoCommand(userId, workoutId, photoId), cancellationToken);
    }
}

public class AddPhotoToWorkoutForm
{
    public IFormFile File { get; set; } = default!;
}

