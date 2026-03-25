using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.Auth;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Application.Extensions;
using FitnessTracking.Shared.Errors;
using Microsoft.AspNetCore.Identity;

namespace FitnessTracking.Application.Features.Commands.RegisterUser;

public class RegisterCommandHandler(
    ITokenService tokenService,
    UserManager<IdentityUser> userManager)
    : ICommandHandler<RegisterUserCommand, Result<RegisterUserResponse, List<Error>>>
{
    public async Task<Result<RegisterUserResponse, List<Error>>> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = new IdentityUser { Email = request.Email, UserName = request.Email };
        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return Result.Failure<RegisterUserResponse, List<Error>>(result.Errors.ToErrors());
        }

        return Result.Success<RegisterUserResponse, List<Error>>(new RegisterUserResponse(user.Id));
    }
}