using System.Text.Json.Nodes;
using FitnessTracking.Application.Features.Commands.CreateWorkout;
using FitnessTracking.Application.Features.Commands.DeleteWorkout;
using FitnessTracking.Application.Features.Commands.PatchWorkout;
using FitnessTracking.Application.Features.Commands.UpdateWorkout;
using FitnessTracking.Application.Features.Queries.GetWorkoutById;
using FitnessTracking.Application.Features.Queries.GetWorkoutsByUserId;
using FitnessTracking.Application.Filters;
using FitnessTracking.Application.Pagination;
using FitnessTracking.Application.Sorting;
using FitnessTracking.Shared.Contracts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracking.Api.Controllers;

[ApiController]
public class WorkoutsController(ISender sender) : ApiControllerBase(sender)
{
    [HttpGet("users/{userId:guid}/workouts")]
    [AllowAnonymous]
    [EndpointSummary("Get user workouts")]
    [EndpointDescription("Returns workouts for a user with filtering, sorting, and pagination.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByUserId(
        Guid userId,
        [FromQuery] WorkoutFilter filter,
        [FromQuery] SortParameters sortParameters,
        [FromQuery] PageParameters pageParameters,
        CancellationToken cancellationToken)
    {
        return await Send(new GetWorkoutsByUserIdQuery(userId, filter, sortParameters, pageParameters), cancellationToken);
    }

    [HttpGet("workouts/{workoutId:guid}")]
    [AllowAnonymous]
    [EndpointSummary("Get workout by id")]
    [EndpointDescription("Returns a single workout by its identifier.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid workoutId, CancellationToken cancellationToken)
    {
        return await Send(new GetWorkoutByIdQuery(workoutId), cancellationToken);
    }

    [HttpPost("workouts")]
    [Authorize]
    [EndpointSummary("Create workout")]
    [EndpointDescription("Creates a new workout for the current user.")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] CreateWorkoutDto dto, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized();
        }

        var command = new CreateWorkoutCommand(userId, dto);
        return await Send(command, cancellationToken, created => Created($"/workouts/{created.WorkoutId}", created));
    }

    [HttpPut("workouts/{workoutId:guid}")]
    [Authorize]
    [EndpointSummary("Update workout")]
    [EndpointDescription("Fully replaces workout data.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid workoutId, [FromBody] UpdateWorkoutDto dto, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized();
        }

        return await Send(new UpdateWorkoutCommand(userId, workoutId, dto), cancellationToken);
    }

    [HttpPatch("workouts/{workoutId:guid}")]
    [Authorize]
    [Consumes("application/merge-patch+json")]
    [EndpointSummary("Patch workout")]
    [EndpointDescription("Applies a JSON Merge Patch document to a workout.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Patch(
        Guid workoutId,
        [FromBody] JsonObject? patch,
        CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized();
        }

        return await Send(new PatchWorkoutCommand(userId, workoutId, RequirePatch(patch)), cancellationToken);
    }

    [HttpDelete("workouts/{workoutId:guid}")]
    [Authorize]
    [EndpointSummary("Delete workout")]
    [EndpointDescription("Deletes a workout by its identifier.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid workoutId, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized();
        }

        return await Send(new DeleteWorkoutCommand(userId, workoutId), cancellationToken);
    }
}

