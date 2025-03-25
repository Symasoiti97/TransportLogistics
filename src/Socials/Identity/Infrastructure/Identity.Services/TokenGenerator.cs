using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using TL.Socials.Identity.Application.UseCases.UserServices;
using TL.Socials.Identity.Infrastructure.Services.Options;

namespace TL.Socials.Identity.Infrastructure.Services;

internal sealed class TokenGenerator(JwtTokenOptions options) : ITokenGenerator
{
    public UserTokens Generate(Guid userId)
    {
        return new UserTokens(userId, GenerateJwtToken(userId), Guid.NewGuid().ToString());
    }

    private string GenerateJwtToken(Guid userId)
    {
        var claims = new[]
        {
            new Claim("user-id", userId.ToString())
        };

        var bytes = Convert.FromBase64String(options.SecurityKey);
        var key = new SymmetricSecurityKey(bytes);
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            options.Issuer,
            options.Audience,
            claims,
            expires: DateTime.Now.Add(options.ExpiresTime),
            signingCredentials: credentials
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }
}