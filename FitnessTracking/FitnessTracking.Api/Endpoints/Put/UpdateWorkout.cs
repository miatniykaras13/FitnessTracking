using Carter;
using FitnessTracking.Api.Extensions;
using FitnessTracking.Application.Abstractions;
using FitnessTracking.Shared.Contracts.Dtos;
using FitnessTracking.Shared.Contracts.Requests;

namespace FitnessTracking.Api.Endpoints.Put;

public class UpdateWorkout : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPut("/workouts/{workoutId:guid}", async (
            Guid workoutId, 
            UpdateWorkoutDto dto,
            HttpContext httpContext,
            IWorkoutsService workoutsService,
            CancellationToken ct = default) =>
        {
            var request = new UpdateWorkoutRequest(
                workoutId,
                Guid.NewGuid(), // todo: брать из claims principal
                dto);
            
            var response = await workoutsService.UpdateAsync(request, ct);
            return response.ToHttpResult(httpContext);
        });
}


