using TL.SharedKernel.Application.Commands;
using TL.SharedKernel.Business.Aggregates;
using TL.Socials.Identity.Application.UseCases.UserServices;
using TL.Socials.Identity.Business.Aggregates.UserSessionAggregate;

namespace TL.Socials.Identity.Application.UseCases.UserSessionServices;

internal sealed class LoginUserViaPhoneCommandHandler(
    IUserRepository userRepository,
    IUserSessionRepository userSessionRepository,
    IAuthTokenGenerator authTokenGenerator)
    : IUseCaseHandler<LoginUserViaPhoneCommand, UserTokens>
{
    public async Task<UserTokens> HandleAsync(LoginUserViaPhoneCommand command, CancellationToken cancellationToken)
    {
        var user = await userRepository.FindAsync(command.PhoneNumber, cancellationToken).ConfigureAwait(false);

        if (user is null)
        {
            throw new Conflict().WithDetails("Invalid phone number.");
        }

        var tokens = authTokenGenerator.Generate(user.Id);
        var userSession = UserSession.Create(user.Id, tokens.RefreshToken);

        await userSessionRepository.SaveAsync(userSession, cancellationToken);

        return tokens;
    }
}