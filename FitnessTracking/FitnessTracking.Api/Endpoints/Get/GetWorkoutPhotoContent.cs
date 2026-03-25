using Carter;
using FitnessTracking.Api.Extensions;
using FitnessTracking.Application.Features.Queries.GetWorkoutPhoto;
using MediatR;

namespace FitnessTracking.Api.Endpoints.Get;

public class GetWorkoutPhotoContent : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet("/workouts/{workoutId:guid}/photos/{photoId:guid}/content", async (
            Guid workoutId,
            Guid photoId,
            HttpContext httpContext,
            ISender sender,
            CancellationToken ct = default) =>
        {
            var query = new GetWorkoutPhotoQuery(workoutId, photoId);
            var response = await sender.Send(query, ct);
            return response.ToHttpResult(httpContext, success => Results.Redirect(success.Path));
        })
        .WithTags("Workouts")
        .WithName("GetWorkoutPhotoContent")
        .WithSummary("Get workout photo content")
        .WithDescription("Redirects to the static URL of the workout photo file.")
        .Produces(StatusCodes.Status302Found)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithOpenApi();
}

