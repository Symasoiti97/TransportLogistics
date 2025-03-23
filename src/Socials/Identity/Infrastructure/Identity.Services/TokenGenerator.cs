using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TL.Socials.Identity.Application.UseCases.UserServices;

namespace TL.Socials.Identity.Infrastructure.Services;

internal sealed class TokenGenerator : ITokenGenerator
{
    public UserTokens Generate(Guid userId)
    {
        return new UserTokens(GenerateJwtToken(userId), Guid.NewGuid().ToString());
    }

    private static string GenerateJwtToken(Guid userId)
    {
        var claims = new[]
        {
            new Claim("user-id", userId.ToString())
        };

        var secretKey = "your-256-bit-secret";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "your-issuer",
            audience: "your-audience",
            claims: claims,
            expires: DateTime.Now.AddMinutes(30), // Токен истекает через 30 минут
            signingCredentials: credentials
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }
}