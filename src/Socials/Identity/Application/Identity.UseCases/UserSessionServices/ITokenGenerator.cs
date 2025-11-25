namespace TL.Socials.Identity.Application.UseCases.UserSessionServices;

public interface IAuthTokenGenerator
{
    UserTokens Generate(Guid userId);
}
