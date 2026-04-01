using FitnessTracking.Application.Features.Commands.AddExercise;
using FitnessTracking.Application.Features.Commands.DeleteExercise;
using FitnessTracking.Application.Features.Commands.UpdateExercise;
using FitnessTracking.Application.Features.Commands.UpdateExercises;
using FitnessTracking.Application.Features.Queries.GetExercisesByWorkoutId;
using FitnessTracking.Shared.Contracts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracking.Api.Controllers;

[ApiController]
public class ExercisesController(ISender sender) : ApiControllerBase(sender)
{
    [HttpGet("workouts/{workoutId:guid}/exercises")]
    [AllowAnonymous]
    [EndpointSummary("Get workout exercises")]
    [EndpointDescription("Returns exercises and sets for the specified workout.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByWorkout(Guid workoutId, CancellationToken cancellationToken)
    {
        return await Send(new GetExercisesByWorkoutIdQuery(workoutId), cancellationToken);
    }

    [HttpPost("workouts/{workoutId:guid}/exercises")]
    [Authorize]
    [EndpointSummary("Add exercise")]
    [EndpointDescription("Adds a new exercise to the workout.")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Add(Guid workoutId, [FromBody] AddExerciseDto dto, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized();
        }

        var command = new AddExerciseCommand(userId, workoutId, dto);
        return await Send(command, cancellationToken, created => Created($"/workouts/{workoutId}/exercises/{Uri.EscapeDataString(created.Name)}", created));
    }


    [HttpPut("workouts/{workoutId:guid}/exercises")]
    [Authorize]
    [EndpointSummary("Replace workout exercises")]
    [EndpointDescription("Replaces the full exercise list for the specified workout.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateAll(Guid workoutId, [FromBody] UpdateExercisesDto dto, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized();
        }

        return await Send(new UpdateExercisesCommand(userId, workoutId, dto), cancellationToken);
    }

    [HttpDelete("workouts/{workoutId:guid}/exercises/{exerciseName}")]
    [Authorize]
    [EndpointSummary("Delete exercise")]
    [EndpointDescription("Deletes an exercise from a workout by name.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid workoutId, string exerciseName, CancellationToken cancellationToken)
    {
        if (!TryGetCurrentUserId(out var userId))
        {
            return Unauthorized();
        }

        return await Send(new DeleteExerciseCommand(userId, workoutId, exerciseName), cancellationToken);
    }
}

