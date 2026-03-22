using Carter;
using FitnessTracking.Application.Abstractions;
using FitnessTracking.Api.Extensions;
using FitnessTracking.Shared.Contracts.Requests;

namespace FitnessTracking.Api.Endpoints;

public class GetWorkoutsByUserId : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet("/users/{userId:guid}/workouts", async (
            Guid userId,
            HttpContext httpContext,
            IWorkoutsService workoutsService,
            CancellationToken ct = default) =>
        {
            var request = new GetWorkoutsByUserIdRequest(userId);
            var response = await workoutsService.GetByUserIdAsync(request, ct);
            return response.ToHttpResult(httpContext);
        });
}


