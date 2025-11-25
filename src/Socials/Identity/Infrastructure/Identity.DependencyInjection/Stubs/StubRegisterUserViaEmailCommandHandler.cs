using TL.SharedKernel.Application.Commands;
using TL.Socials.Identity.Application.UseCases.UserServices;
using TL.Socials.Identity.Business.Aggregates.UserAggregate;

namespace TL.Socials.Identity.Infrastructure.DependencyInjection.Stubs;

internal sealed class StubRegisterUserViaEmailCommandHandler(
    IUserRepository userRepository,
    RegisterUserViaEmailCommandHandler originHandler)
    : IUseCaseHandler<RegisterUserViaEmailCommand>
{
    public async Task HandleAsync(RegisterUserViaEmailCommand command, CancellationToken cancellationToken)
    {
        if (command.Token != Guid.Empty.ToString())
        {
            await originHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            var user = User.Create(command.Email, command.Password);

            await userRepository.AddAsync(user, cancellationToken).ConfigureAwait(false);
        }
    }
}
