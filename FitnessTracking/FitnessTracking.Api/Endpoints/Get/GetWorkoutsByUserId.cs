using Carter;
using FitnessTracking.Api.Extensions;
using FitnessTracking.Application.Abstractions;
using FitnessTracking.Shared.Contracts.Requests;

namespace FitnessTracking.Api.Endpoints.Get;

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


