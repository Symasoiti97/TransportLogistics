namespace TL.Socials.Identity.Application.UseCases.UserServices;

public sealed class UserTokens(Guid userId, string accessToken, string refreshToken)
{
    public Guid UserId { get; } = userId;
    public string AccessToken { get; } = accessToken;
    public string RefreshToken { get; } = refreshToken;
}