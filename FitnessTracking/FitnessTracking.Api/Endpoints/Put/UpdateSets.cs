using Carter;
using FitnessTracking.Api.Extensions;
using FitnessTracking.Application.Abstractions;
using FitnessTracking.Shared.Contracts.Dtos;
using FitnessTracking.Shared.Contracts.Requests;

namespace FitnessTracking.Api.Endpoints.Put;

public class UpdateSets : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPut("/workouts/{workoutId:guid}/exercises/{exerciseName}/sets", async (
            Guid workoutId,
            string exerciseName,
            UpdateSetsDto dto,
            HttpContext httpContext,
            IWorkoutsService workoutsService,
            CancellationToken ct = default) =>
        {
            var request = new UpdateSetsRequest(workoutId, exerciseName, dto);
            var result = await workoutsService.UpdateSetsAsync(request, ct);
            return result.ToHttpResult(httpContext);
        });
}


