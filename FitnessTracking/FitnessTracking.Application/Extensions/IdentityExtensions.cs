using FitnessTracking.Shared.Errors;
using Microsoft.AspNetCore.Identity;

namespace FitnessTracking.Application.Extensions;

public static class IdentityExtensions
{
    public static Error ToError(this IdentityError identityError)
    {
        return identityError.Code switch
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
        };
    }

    public static List<Error> ToErrors(this IEnumerable<IdentityError> identityErrors) =>
        identityErrors.Select(e => e.ToError()).ToList();
}