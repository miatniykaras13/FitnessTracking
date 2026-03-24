using Carter;
using FitnessTracking.Api.Extensions;
using FitnessTracking.Application.Abstractions;
using FitnessTracking.Shared.Contracts.Requests;

namespace FitnessTracking.Api.Endpoints.Get;

public class GetWorkoutById : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet("/workouts/{workoutId:guid}", async (
            Guid workoutId,
            HttpContext httpContext,
            IWorkoutsService workoutsService,
            CancellationToken ct = default) =>
        {
            var request = new GetWorkoutByIdRequest(workoutId);
            var workoutResponse = await workoutsService.GetByIdAsync(request, ct);
            return workoutResponse.ToHttpResult(httpContext);
        })
        .WithTags("Workouts")
        .WithName("GetWorkoutById")
        .WithSummary("Get workout by id")
        .WithDescription("Returns a single workout with all basic workout fields.")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithOpenApi();
}