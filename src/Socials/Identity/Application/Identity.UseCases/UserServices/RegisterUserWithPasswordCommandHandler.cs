using TL.SharedKernel.Application.Commands;
using TL.Socials.Identity.Business.Aggregates.UserAggregate;

namespace TL.Socials.Identity.Application.UseCases.UserServices;

internal sealed class RegisterUserWithPasswordCommandHandler(IUserRepository userRepository)
    : ICommandHandler<RegisterUserWithPasswordCommand>
{
    public async Task HandleAsync(RegisterUserWithPasswordCommand command, CancellationToken cancellationToken)
    {
        var user = User.Create(command.Email, command.Password);

        await userRepository.AddAsync(user, cancellationToken).ConfigureAwait(false);
    }
}