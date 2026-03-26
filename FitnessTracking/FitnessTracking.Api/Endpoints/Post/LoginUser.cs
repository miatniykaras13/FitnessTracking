using Carter;
using FitnessTracking.Api.Extensions;
using FitnessTracking.Application.Features.Commands.LoginUser;
using FitnessTracking.Shared.Contracts;
using MediatR;

namespace FitnessTracking.Api.Endpoints.Post;

public class LoginUser : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPost("/login", async (
                LoginDto dto,
                HttpContext httpContext,
                ISender sender,
                CancellationToken ct = default) =>
            {
                var command = new LoginUserCommand(dto.Email, dto.Password);

                var response = await sender.Send(command, ct);
                return response.ToHttpResult(httpContext);
            })
            .WithTags("Auth")
            .WithName("LoginUser")
            .WithSummary("Login user")
            .WithDescription("Authenticates user by email and password and returns an access token.")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .AllowAnonymous()
            .WithOpenApi();
}