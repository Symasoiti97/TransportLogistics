using TL.SharedKernel.Application.Commands;
using TL.SharedKernel.Business.Aggregates;

namespace TL.Socials.Identity.Application.UseCases.UserServices;

internal sealed class RefreshAuthTokenCommandHandler(
    IUserSessionRepository userSessionRepository,
    ITokenGenerator tokenGenerator)
    : IUseCaseHandler<RefreshAuthTokenCommand, UserTokens>
{
    public async Task<UserTokens> HandleAsync(RefreshAuthTokenCommand command, CancellationToken cancellationToken)
    {
        var userSession = await userSessionRepository.FindAsync(command.RefreshToken, cancellationToken);

        if (userSession is null)
        {
            throw new Conflict().WithDetails("Refresh token not found.");
        }

        var tokens = tokenGenerator.Generate(userSession.UserId);
        userSession.UpdateRefreshToken(tokens.RefreshToken);

        await userSessionRepository.SaveAsync(userSession, cancellationToken);

        return tokens;
    }
}