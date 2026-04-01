using FitnessTracking.Application.Features.Commands.LoginUser;
using FitnessTracking.Application.Features.Commands.RegisterUser;
using FitnessTracking.Shared.Contracts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracking.Api.Controllers;

[ApiController]
public class AuthController(ISender sender) : ApiControllerBase(sender)
{
    [HttpPost("users")]
    [AllowAnonymous]
    [EndpointSummary("Register user")]
    [EndpointDescription("Creates a new user using email and password.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto, CancellationToken cancellationToken)
    {
        return await Send(new RegisterUserCommand(dto.Email, dto.Password), cancellationToken);
    }

    [HttpPost("auth/token")]
    [AllowAnonymous]
    [EndpointSummary("Get JWT token")]
    [EndpointDescription("Authenticates a user and returns an access token.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken cancellationToken)
    {
        return await Send(new LoginUserCommand(dto.Email, dto.Password), cancellationToken);
    }
}
