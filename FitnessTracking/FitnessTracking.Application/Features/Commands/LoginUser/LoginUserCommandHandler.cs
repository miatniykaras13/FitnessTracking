using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.Auth;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Commands.LoginUser;

public class LoginUserCommandHandler(
    ITokenService tokenService,
    IAuthService authService)
    : ICommandHandler<LoginUserCommand, Result<LoginUserResponse, List<Error>>>
{
    public async Task<Result<LoginUserResponse, List<Error>>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await authService.ValidateCredentialsAsync(request.Email, request.Password, cancellationToken);

        if (user is null)
        {
            return Result.Failure<LoginUserResponse, List<Error>>(Error.Unauthorized());
        }

        return Result.Success<LoginUserResponse, List<Error>>(new LoginUserResponse(tokenService.GenerateToken(user)));
    }
}
