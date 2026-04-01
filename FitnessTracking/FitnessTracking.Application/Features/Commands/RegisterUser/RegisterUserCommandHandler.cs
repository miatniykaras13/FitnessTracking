using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.Auth;
using FitnessTracking.Application.Abstractions.CQRS;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Features.Commands.RegisterUser;

public class RegisterUserCommandHandler(IAuthService authService)
    : ICommandHandler<RegisterUserCommand, Result<RegisterUserResponse, List<Error>>>
{
    public async Task<Result<RegisterUserResponse, List<Error>>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var registerResult = await authService.RegisterAsync(request.Email, request.Password, cancellationToken);

        if (registerResult.IsFailure)
        {
            return Result.Failure<RegisterUserResponse, List<Error>>(registerResult.Error);
        }

        return Result.Success<RegisterUserResponse, List<Error>>(new RegisterUserResponse(registerResult.Value.Id));
    }
}
