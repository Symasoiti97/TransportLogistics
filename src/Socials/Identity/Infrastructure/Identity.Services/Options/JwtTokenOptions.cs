namespace TL.Socials.Identity.Infrastructure.Services.Options;

public sealed class JwtTokenOptions
{
    public string SecurityKey { get; }
    public string Issuer { get; }
    public string Audience { get; }
    public TimeSpan ExpiresTime { get; }

    public JwtTokenOptions(
        string securityKey,
        string issuer,
        string audience,
        TimeSpan expiresTime)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(securityKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(issuer);
        ArgumentException.ThrowIfNullOrWhiteSpace(audience);

        SecurityKey = securityKey;
        Issuer = issuer;
        Audience = audience;
        ExpiresTime = expiresTime;
    }
}
