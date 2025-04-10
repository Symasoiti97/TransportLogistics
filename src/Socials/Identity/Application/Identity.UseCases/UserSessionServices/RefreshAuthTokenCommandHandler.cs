using TL.SharedKernel.Application.Commands;
using TL.SharedKernel.Business.Aggregates;

namespace TL.Socials.Identity.Application.UseCases.UserSessionServices;

internal sealed class RefreshAuthTokenCommandHandler(
    IUserSessionRepository userSessionRepository,
    IAuthTokenGenerator authTokenGenerator)
    : IUseCaseHandler<RefreshAuthTokenCommand, UserTokens>
{
    public async Task<UserTokens> HandleAsync(RefreshAuthTokenCommand command, CancellationToken cancellationToken)
    {
        var userSession = await userSessionRepository.FindAsync(command.RefreshToken, cancellationToken);

        if (userSession is null)
        {
            throw new Conflict().WithDetails("Refresh token not found.");
        }

        var tokens = authTokenGenerator.Generate(userSession.UserId);
        userSession.UpdateRefreshToken(tokens.RefreshToken);

        await userSessionRepository.SaveAsync(userSession, cancellationToken);

        return tokens;
    }
}