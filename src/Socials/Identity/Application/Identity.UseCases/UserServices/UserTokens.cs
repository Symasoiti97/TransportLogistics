namespace TL.Socials.Identity.Application.UseCases.UserServices;

public sealed class UserTokens(string accessToken, string refreshToken)
{
    public Guid UserId { get; set; }
    public string AccessToken { get; } = accessToken;
    public string RefreshToken { get; } = refreshToken;
}