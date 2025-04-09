using TL.SharedKernel.Application.Commands;
using TL.SharedKernel.Business.Aggregates;
using TL.Socials.Identity.Business.Aggregates.TokenAggregate;

namespace TL.Socials.Identity.Application.UseCases.UserServices;

internal sealed class RequestUserRegisterViaEmailCommandHandler(
    IUserRepository userRepository,
    IUserRegisterViaEmailTokenRepository tokenRepository)
    : IUseCaseHandler<RequestUserRegisterViaEmailCommand, bool>
{
    public async Task<bool> HandleAsync(RequestUserRegisterViaEmailCommand command, CancellationToken cancellationToken)
    {
        var user = await userRepository.FindAsync(command.Email, cancellationToken);

        if (user is not null)
        {
            return false;
        }

        var token = await tokenRepository.FindAsync(command.Email, cancellationToken);

        if (token is not null)
        {
            throw new Conflict().WithDetails("User register request already exists.");
        }

        await tokenRepository.SaveAsync(UserRegisterViaEmailToken.Create(command.Email), cancellationToken);

        return true;
    }
}