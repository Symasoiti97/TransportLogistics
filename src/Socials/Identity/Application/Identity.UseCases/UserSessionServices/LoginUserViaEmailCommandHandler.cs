using TL.SharedKernel.Application.Commands;
using TL.SharedKernel.Business.Aggregates;
using TL.Socials.Identity.Application.UseCases.UserServices;
using TL.Socials.Identity.Business.Aggregates.UserSessionAggregate;

namespace TL.Socials.Identity.Application.UseCases.UserSessionServices;

internal sealed class LoginUserViaEmailCommandHandler(
    IUserRepository userRepository,
    IUserSessionRepository userSessionRepository,
    IAuthTokenGenerator authTokenGenerator)
    : IUseCaseHandler<LoginUserViaEmailCommand, UserTokens>
{
    public async Task<UserTokens> HandleAsync(LoginUserViaEmailCommand command, CancellationToken cancellationToken)
    {
        var user = await userRepository.FindAsync(command.Email, cancellationToken).ConfigureAwait(false);

        if (user is null || !user.IsPasswordValid(command.Password))
        {
            throw new Conflict().WithDetails("Email address not found or password is incorrect.");
        }

        var tokens = authTokenGenerator.Generate(user.Id);
        var userSession = UserSession.Create(user.Id, tokens.RefreshToken);

        await userSessionRepository.SaveAsync(userSession, cancellationToken);

        return tokens;
    }
}