using Carter;
using FitnessTracking.Api.Extensions;
using FitnessTracking.Application.Features.Commands.RegisterUser;
using FitnessTracking.Shared.Contracts;
using MediatR;

namespace FitnessTracking.Api.Endpoints.Post;

public class RegisterUser : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPost("/register", async (
                RegisterDto dto,
                HttpContext httpContext,
                ISender sender,
                CancellationToken ct = default) =>
            {
                var command = new RegisterUserCommand(dto.Email, dto.Password);

                var response = await sender.Send(command, ct);
                return response.ToHttpResult(httpContext);
            })
            .WithTags("Auth")
            .WithName("RegisterUser")
            .WithSummary("Register user")
            .WithDescription("Registers a new user with email and password.")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .AllowAnonymous()
            .WithOpenApi();
}