using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FitnessTracking.Application.Abstractions.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FitnessTracking.Infrastructure.Services;

public class TokenService(IConfiguration configuration) : ITokenService
{
    public string GenerateToken(IdentityUser user)
    {
        var jwtSettings = configuration.GetSection("Auth");

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email ?? "")
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings["Secret"] ??
                                   throw new InvalidOperationException("Auth secret must be provided")));

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"] ?? throw new InvalidOperationException("Auth issuer must be provided"),
            audience: jwtSettings["Audience"] ?? throw new InvalidOperationException("Auth audience must be provided"),
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                double.Parse(jwtSettings["ExpiresInMinutes"] ?? "5")),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}