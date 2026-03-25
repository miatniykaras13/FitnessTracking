using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.Auth;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Shared.Errors;
using Microsoft.AspNetCore.Identity;

namespace FitnessTracking.Application.Features.Commands.LoginUser;

public class LoginUserCommandHandler(
    ITokenService tokenService,
    UserManager<IdentityUser> userManager)
    : ICommandHandler<LoginUserCommand, Result<LoginUserResponse, List<Error>>>
{
    public async Task<Result<LoginUserResponse, List<Error>>> Handle(
        LoginUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        
        if(user is null || !await userManager.CheckPasswordAsync(user, request.Password))
            return Result.Failure<LoginUserResponse, List<Error>>(Error.Unauthorized());
        
        return Result.Success<LoginUserResponse, List<Error>>(new LoginUserResponse(tokenService.GenerateToken(user)));
    }
}

