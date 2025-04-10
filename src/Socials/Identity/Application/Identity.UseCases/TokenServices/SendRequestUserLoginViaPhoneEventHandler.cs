using Microsoft.Extensions.Logging;
using TL.SharedKernel.Application.Commands;
using TL.SharedKernel.Business.Aggregates;
using TL.Socials.Identity.Business.Aggregates.TokenAggregate;

namespace TL.Socials.Identity.Application.UseCases.TokenServices;

internal sealed class SendRequestUserLoginViaPhoneEventHandler(
    IUserLoginViaPhoneTokenRepository tokenRepository,
    ILogger<SendRequestUserRegisterEmailEventHandler> logger)
    : IUseCaseHandler<UserLoginViaPhoneTokenCreated>
{
    public async Task HandleAsync(UserLoginViaPhoneTokenCreated command, CancellationToken cancellationToken)
    {
        var token = await tokenRepository.FindAsync(command.TokenId, cancellationToken).ConfigureAwait(false);
        if (token is null)
        {
            throw new Conflict().WithDetails("Token not found.");
        }

        logger.LogDebug(
            "Sent phone notification of request user login with phone/token: {Phone}/{TokenValue}",
            token.PhoneNumber.Value,
            token.Value);
    }
}