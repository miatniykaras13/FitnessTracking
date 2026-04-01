using CSharpFunctionalExtensions;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Errors;

namespace FitnessTracking.Application.Abstractions.Auth;

public interface IAuthService
{
    Task<Result<AuthUser, List<Error>>> RegisterAsync(string email, string password, CancellationToken cancellationToken);

    Task<AuthUser?> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken);
}

