using TL.SharedKernel.Application.Commands;
using TL.Socials.Identity.Application.UseCases.UserServices;
using TL.Socials.Identity.Business.Aggregates.UserAggregate;

namespace TL.Socials.Identity.Infrastructure.DependencyInjection.Stubs;

internal sealed class StubRegisterUserViaPhoneCommandHandler(
    IUserRepository userRepository,
    RegisterUserViaPhoneCommandHandler originHandler)
    : IUseCaseHandler<RegisterUserViaPhoneCommand>
{
    public async Task HandleAsync(RegisterUserViaPhoneCommand command, CancellationToken cancellationToken)
    {
        if (command.Code != "000000")
        {
            await originHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            var user = User.Create(command.PhoneNumber);

            await userRepository.AddAsync(user, cancellationToken).ConfigureAwait(false);
        }
    }
}
