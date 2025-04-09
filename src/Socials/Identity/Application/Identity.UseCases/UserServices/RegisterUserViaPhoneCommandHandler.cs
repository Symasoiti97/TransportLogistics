using TL.SharedKernel.Application.Commands;
using TL.SharedKernel.Business.Aggregates;
using TL.Socials.Identity.Business.Aggregates.UserAggregate;

namespace TL.Socials.Identity.Application.UseCases.UserServices;

internal sealed class RegisterUserViaPhoneCommandHandler(
    IUserRepository userRepository,
    IUserLoginViaPhoneTokenRepository tokenRepository)
    : IUseCaseHandler<RegisterUserViaPhoneCommand>
{
    public async Task HandleAsync(RegisterUserViaPhoneCommand command, CancellationToken cancellationToken)
    {
        var token = await tokenRepository.FindAsync(command.Code, cancellationToken);

        if (token is null || !token.PhoneNumber.Equals(command.PhoneNumber))
        {
            throw new Conflict().WithDetails("Invalid code.");
        }

        var user = await userRepository.FindAsync(command.PhoneNumber, cancellationToken);

        if (user is null)
        {
            await userRepository.AddAsync(User.Create(command.PhoneNumber), cancellationToken);
        }
    }
}