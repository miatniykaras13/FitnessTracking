using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Globalization;
using System.Text;
using FitnessTracking.Application.Abstractions.Auth;
using FitnessTracking.Shared.Constants;
using FitnessTracking.Shared.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FitnessTracking.Infrastructure.Services;

public class TokenService(IConfiguration configuration) : ITokenService
{
    public string GenerateToken(IdentityUser user)
    {
        var jwtSettings = configuration.GetSection(AuthConstants.SectionName);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email ?? "")
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings[AuthConstants.SecretKey] ??
                                   throw new MissingConfigurationException(AuthConstants.SecretPath)));

        var tokenLifetimeMinutes = double.TryParse(
            jwtSettings[AuthConstants.ExpiresInMinutesKey],
            NumberStyles.Float,
            CultureInfo.InvariantCulture,
            out var parsedMinutes)
            ? parsedMinutes
            : AuthConstants.DefaultTokenLifetimeMinutes;

        var token = new JwtSecurityToken(
            issuer: jwtSettings[AuthConstants.IssuerKey] ?? throw new MissingConfigurationException(AuthConstants.IssuerPath),
            audience: jwtSettings[AuthConstants.AudienceKey] ?? throw new MissingConfigurationException(AuthConstants.AudiencePath),
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(tokenLifetimeMinutes),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}