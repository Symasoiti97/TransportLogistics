using TL.Socials.Identity.Business.Aggregates.UserAggregate;

namespace TL.Socials.Identity.Application.UseCases.UserServices;

public interface ITokenGenerator
{
    UserTokens Generate(Email email);
}