using TL.SharedKernel.Application.Commands;
using TL.Socials.Identity.Business.Aggregates.TokenAggregate;

namespace TL.Socials.Identity.Application.UseCases.TokenServices;

internal sealed class RequestUserLoginViaPhoneCommandHandler(
    IUserLoginViaPhoneTokenRepository tokenRepository)
    : IUseCaseHandler<RequestUserLoginViaPhoneCommand>
{
    public async Task HandleAsync(RequestUserLoginViaPhoneCommand command, CancellationToken cancellationToken)
    {
        var token = await tokenRepository.FindAsync(command.PhoneNumber, cancellationToken);

        if (token is not null)
        {
            token.UpdateTokenValue();
        }
        else
        {
            token = UserLoginViaPhoneToken.Create(command.PhoneNumber);
        }

        await tokenRepository.SaveAsync(token, cancellationToken);
    }
}