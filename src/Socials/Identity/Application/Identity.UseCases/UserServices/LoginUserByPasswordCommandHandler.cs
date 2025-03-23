using TL.SharedKernel.Application.Commands;
using TL.SharedKernel.Business.Aggregates;
using TL.Socials.Identity.Business.Aggregates.UserSessionAggregate;

namespace TL.Socials.Identity.Application.UseCases.UserServices;

internal sealed class LoginUserByPasswordCommandHandler(
    IUserRepository userRepository,
    IUserSessionRepository userSessionRepository,
    ITokenGenerator tokenGenerator)
    : IQueryHandler<LoginUserByPasswordCommand, UserTokens>
{
    public async Task<UserTokens> HandleAsync(LoginUserByPasswordCommand command, CancellationToken cancellationToken)
    {
        var user = await userRepository.FindAsync(command.Email, cancellationToken).ConfigureAwait(false);

        if (user is null || !user.IsPasswordValid(command.Password))
        {
            throw new Conflict().WithDetails("Email address not found or password is incorrect.");
        }

        var tokens = tokenGenerator.Generate(user.Id);
        var userSession = UserSession.Create(user.Id, tokens.RefreshToken);

        await userSessionRepository.AddAsync(userSession, cancellationToken);

        return tokens;
    }
}