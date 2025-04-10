using Microsoft.Extensions.Logging;
using TL.SharedKernel.Application.Commands;
using TL.SharedKernel.Business.Aggregates;
using TL.Socials.Identity.Business.Aggregates.TokenAggregate;

namespace TL.Socials.Identity.Application.UseCases.TokenServices;

internal sealed class SendRequestUserRegisterEmailEventHandler(
    IUserRegisterViaEmailTokenRepository tokenRepository,
    ILogger<SendRequestUserRegisterEmailEventHandler> logger)
    : IUseCaseHandler<UserRegisterViaEmailTokenCreated>
{
    public async Task HandleAsync(UserRegisterViaEmailTokenCreated command, CancellationToken cancellationToken)
    {
        var token = await tokenRepository.FindAsync(command.TokenId, cancellationToken).ConfigureAwait(false);
        if (token is null)
        {
            throw new Conflict().WithDetails("Token not found.");
        }

        logger.LogDebug(
            "Sent email notification of request user register with email/token: {Email}/{TokenValue}",
            token.Email.Value,
            token.Value);
    }
}