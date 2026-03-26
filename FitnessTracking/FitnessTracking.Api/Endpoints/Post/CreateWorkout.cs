using System.Security.Claims;
using Carter;
using FitnessTracking.Api.Extensions;
using FitnessTracking.Application.Features.Commands.CreateWorkout;
using FitnessTracking.Shared.Contracts;
using MediatR;

namespace FitnessTracking.Api.Endpoints.Post;

public class CreateWorkout : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPost("/workouts", async (
                CreateWorkoutDto dto,
                ClaimsPrincipal user,
                HttpContext httpContext,
                ISender sender,
                CancellationToken ct = default) =>
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId is null)
                    return Results.Unauthorized();

                var command = new CreateWorkoutCommand(
                    Guid.Parse(userId),
                    dto);
                var response = await sender.Send(command, ct);
                return response.ToHttpResult(httpContext,
                    created => Results.Created($"/workouts/{created.WorkoutId}", created));
            })
            .RequireAuthorization()
            .WithTags("Workouts")
            .WithName("CreateWorkout")
            .WithSummary("Create workout")
            .WithDescription("Creates a new workout for the current user.")
            .Produces(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithOpenApi();
}