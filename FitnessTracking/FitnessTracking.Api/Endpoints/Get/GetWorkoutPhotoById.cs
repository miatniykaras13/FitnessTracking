using Carter;
using FitnessTracking.Api.Extensions;
using FitnessTracking.Application.Features.Queries.GetWorkoutPhoto;
using MediatR;

namespace FitnessTracking.Api.Endpoints.Get;

public class GetWorkoutPhotoById : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet("/workouts/{workoutId:guid}/photos/{photoId:guid}", async (
            Guid workoutId,
            Guid photoId,
            HttpContext httpContext,
            ISender sender,
            CancellationToken ct = default) =>
        {
            var query = new GetWorkoutPhotoQuery(workoutId, photoId);
            var response = await sender.Send(query, ct);
            return response.ToHttpResult(httpContext);
        })
        .WithTags("Workouts")
        .WithName("GetWorkoutPhotoById")
        .WithSummary("Get workout photo by id")
        .WithDescription("Returns metadata for a workout photo.")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithOpenApi();
}

