using Microsoft.AspNetCore.Identity;

namespace FitnessTracking.Application.Abstractions.Auth;

public interface ITokenService
{
    string GenerateToken(IdentityUser userId);
}