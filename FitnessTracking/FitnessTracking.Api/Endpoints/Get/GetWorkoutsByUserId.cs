using Carter;
using FitnessTracking.Api.Extensions;
using FitnessTracking.Application.Abstractions;
using FitnessTracking.Application.Filters;
using FitnessTracking.Application.Paging;
using FitnessTracking.Application.Sorting;
using FitnessTracking.Shared.Contracts.Requests;

namespace FitnessTracking.Api.Endpoints.Get;

public class GetWorkoutsByUserId : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet("/users/{userId:guid}/workouts", async (
                Guid userId,
                [AsParameters] WorkoutFilter filter,
                [AsParameters] SortParameters sortParameters,
                [AsParameters] PageParameters pageParameters,
                HttpContext httpContext,
                IWorkoutsService workoutsService,
                CancellationToken ct = default) =>
            {
                var request = new GetWorkoutsByUserIdRequest(userId);
                var response = await workoutsService.GetByUserIdAsync(
                    request,
                    filter,
                    sortParameters,
                    pageParameters,
                    ct);
                return response.ToHttpResult(httpContext);
            })
            .WithTags("Workouts")
            .WithName("GetWorkoutsByUserId")
            .WithSummary("Get user workouts")
            .WithDescription("Returns all workouts created by the specified user.")
            .Produces(StatusCodes.Status200OK)
            .WithOpenApi();
}