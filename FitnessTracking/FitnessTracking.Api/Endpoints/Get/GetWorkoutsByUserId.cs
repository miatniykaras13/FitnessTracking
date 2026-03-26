using Carter;
using FitnessTracking.Api.Extensions;
using FitnessTracking.Application.Features.Queries.GetWorkoutsByUserId;
using FitnessTracking.Application.Filters;
using FitnessTracking.Application.Pagination;
using FitnessTracking.Application.Sorting;
using MediatR;

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
                ISender sender,
                CancellationToken ct = default) =>
            {
                var query = new GetWorkoutsByUserIdQuery(userId, filter, sortParameters, pageParameters);
                var response = await sender.Send(query, ct);
                return response.ToHttpResult(httpContext);
            })
            .WithTags("Workouts")
            .WithName("GetWorkoutsByUserId")
            .WithSummary("Get user workouts")
            .WithDescription("Returns all workouts created by the specified user.")
            .Produces(StatusCodes.Status200OK)
            .AllowAnonymous()
            .WithOpenApi();
}