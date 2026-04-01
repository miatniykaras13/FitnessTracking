using FitnessTracking.Domain.Models;

namespace FitnessTracking.Application.Abstractions.Auth;

public interface ITokenService
{
    string GenerateToken(AuthUser user);
}