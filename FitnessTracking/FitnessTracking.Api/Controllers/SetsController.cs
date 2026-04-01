using System.Text.Json.Nodes;
using FitnessTracking.Application.Features.Commands.AddSet;
using FitnessTracking.Application.Features.Commands.DeleteSet;
using FitnessTracking.Application.Features.Commands.PatchSet;
using FitnessTracking.Application.Features.Commands.UpdateSet;
using FitnessTracking.Application.Features.Commands.UpdateSets;
using FitnessTracking.Shared.Contracts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracking.Api.Controllers;

[ApiController]
public class SetsController(ISender sender) : ApiControllerBase(sender)
{
    [HttpPost("workouts/{workoutId:guid}/exercises/{exerciseName}/sets")]
    [Authorize]
    [EndpointSummary("Add set")]
    [EndpointDescription("Adds a new set to the specified exercise.")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Add(
        Guid workoutId,
        string exerciseName,
        [FromBody] AddSetDto dto,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized();
        }

        var command = new AddSetCommand(userId, workoutId, exerciseName, dto);
        return await Send(command, cancellationToken, created => Created($"/workouts/{workoutId}/exercises/{Uri.EscapeDataString(exerciseName)}/sets", created));
    }

    [HttpPut("workouts/{workoutId:guid}/exercises/{exerciseName}/sets/{setIndex:int}")]
    [Authorize]
    [EndpointSummary("Update set")]
    [EndpointDescription("Updates a single set by index.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid workoutId,
        string exerciseName,
        int setIndex,
        [FromBody] UpdateSetDto dto,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized();
        }

        return await Send(new UpdateSetCommand(userId, workoutId, exerciseName, setIndex, dto), cancellationToken);
    }

    [HttpPut("workouts/{workoutId:guid}/exercises/{exerciseName}/sets")]
    [Authorize]
    [EndpointSummary("Replace exercise sets")]
    [EndpointDescription("Fully replaces the set list for the specified exercise.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAll(
        Guid workoutId,
        string exerciseName,
        [FromBody] UpdateSetsDto dto,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized();
        }

        return await Send(new UpdateSetsCommand(userId, workoutId, exerciseName, dto), cancellationToken);
    }

    [HttpPatch("workouts/{workoutId:guid}/exercises/{exerciseName}/sets/{setIndex:int}")]
    [Authorize]
    [Consumes("application/merge-patch+json")]
    [EndpointSummary("Patch set")]
    [EndpointDescription("Applies a JSON Merge Patch document to a set by index.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Patch(
        Guid workoutId,
        string exerciseName,
        int setIndex,
        [FromBody] JsonObject? patch,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized();
        }

        var command = new PatchSetCommand(userId, workoutId, exerciseName, setIndex, RequirePatch(patch));
        return await Send(command, cancellationToken);
    }

    [HttpDelete("workouts/{workoutId:guid}/exercises/{exerciseName}/sets/{setIndex:int}")]
    [Authorize]
    [EndpointSummary("Delete set")]
    [EndpointDescription("Deletes a set by index.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid workoutId, string exerciseName, int setIndex, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized();
        }

        return await Send(new DeleteSetCommand(userId, workoutId, exerciseName, setIndex), cancellationToken);
    }
}
