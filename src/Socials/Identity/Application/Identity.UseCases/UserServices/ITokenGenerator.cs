namespace TL.Socials.Identity.Application.UseCases.UserServices;

public interface ITokenGenerator
{
    UserTokens Generate(Guid userId);
}