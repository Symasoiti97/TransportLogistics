using TL.SharedKernel.Application.Commands;
using TL.SharedKernel.Business.Aggregates;
using TL.Socials.Identity.Application.UseCases.TokenServices;
using TL.Socials.Identity.Business.Aggregates.UserAggregate;

namespace TL.Socials.Identity.Application.UseCases.UserServices;

internal sealed class RegisterUserViaEmailCommandHandler(
    IUserRepository userRepository,
    IUserRegisterViaEmailTokenRepository tokenRepository)
    : IUseCaseHandler<RegisterUserViaEmailCommand>
{
    public async Task HandleAsync(RegisterUserViaEmailCommand command, CancellationToken cancellationToken)
    {
        var token = await tokenRepository.FindAsync(command.Email, cancellationToken).ConfigureAwait(false);

        if (token?.Value != command.Token)
        {
            throw new Conflict().WithDetails("Email does not match.");
        }

        var user = User.Create(command.Email, command.Password);

        await userRepository.AddAsync(user, cancellationToken).ConfigureAwait(false);
    }
}