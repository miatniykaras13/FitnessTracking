using Carter;
using FitnessTracking.Api.Extensions;
using FitnessTracking.Application.Abstractions;
using FitnessTracking.Shared.Contracts.Dtos;
using FitnessTracking.Shared.Contracts.Requests;

namespace FitnessTracking.Api.Endpoints.Put;

public class UpdateExercises : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPut("/workouts/{workoutId:guid}/exercises", async (
            Guid workoutId,
            UpdateExercisesDto dto,
            HttpContext httpContext,
            IWorkoutsService workoutsService,
            CancellationToken ct = default) =>
        {
            var request = new UpdateExercisesRequest(workoutId, dto);
            var result = await workoutsService.UpdateExercisesAsync(request, ct);
            return result.ToHttpResult(httpContext);
        });
}


