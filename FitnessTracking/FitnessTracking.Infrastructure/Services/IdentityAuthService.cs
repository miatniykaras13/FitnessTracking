using CSharpFunctionalExtensions;
using FitnessTracking.Application.Abstractions.Auth;
using FitnessTracking.Domain.Models;
using FitnessTracking.Shared.Errors;
using Microsoft.AspNetCore.Identity;

namespace FitnessTracking.Infrastructure.Services;

public class IdentityAuthService(UserManager<IdentityUser> userManager) : IAuthService
{
    public async Task<Result<AuthUser, List<Error>>> RegisterAsync(
        string email,
        string password,
        CancellationToken cancellationToken)
    {
        var user = new IdentityUser
        {
            Email = email,
            UserName = email
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            return Result.Failure<AuthUser, List<Error>>(MapIdentityErrors(result.Errors));
        }

        return Result.Success<AuthUser, List<Error>>(new AuthUser(user.Id, user.Email ?? email));
    }

    public async Task<AuthUser?> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return null;
        }

        var isValidPassword = await userManager.CheckPasswordAsync(user, password);
        return isValidPassword ? new AuthUser(user.Id, user.Email ?? email) : null;
    }

    private static List<Error> MapIdentityErrors(IEnumerable<IdentityError> identityErrors)
    {
        return identityErrors.Select(identityError => identityError.Code switch
        {
            nameof(IdentityErrorDescriber.DuplicateUserName) =>
                Error.Conflict("user.username", identityError.Description),

            nameof(IdentityErrorDescriber.DuplicateEmail) =>
                Error.Conflict("user.email", identityError.Description),

            nameof(IdentityErrorDescriber.InvalidUserName) =>
                Error.Validation("user.username", identityError.Description),

            nameof(IdentityErrorDescriber.InvalidEmail) =>
                Error.Validation("user.email", identityError.Description),

            nameof(IdentityErrorDescriber.PasswordMismatch) =>
                Error.Validation("user.password", identityError.Description),

            nameof(IdentityErrorDescriber.PasswordTooShort) =>
                Error.Validation("user.password", identityError.Description),

            nameof(IdentityErrorDescriber.PasswordRequiresDigit) =>
                Error.Validation("user.password", identityError.Description),

            nameof(IdentityErrorDescriber.PasswordRequiresUpper) =>
                Error.Validation("user.password", identityError.Description),

            nameof(IdentityErrorDescriber.PasswordRequiresLower) =>
                Error.Validation("user.password", identityError.Description),

            nameof(IdentityErrorDescriber.PasswordRequiresNonAlphanumeric) =>
                Error.Validation("user.password", identityError.Description),

            _ => Error.Validation($"identity.{identityError.Code}", identityError.Description)
        }).ToList();
    }
}

