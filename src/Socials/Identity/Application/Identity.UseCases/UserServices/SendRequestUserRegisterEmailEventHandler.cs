using Microsoft.Extensions.Logging;
using TL.SharedKernel.Application.Commands;
using TL.Socials.Identity.Business.Aggregates.TokenAggregate;

namespace TL.Socials.Identity.Application.UseCases.UserServices;

internal sealed class SendRequestUserRegisterEmailEventHandler(ILogger<SendRequestUserRegisterEmailEventHandler> logger)
    : IUseCaseHandler<UserRegisterViaEmailTokenCreated>
{
    public Task HandleAsync(UserRegisterViaEmailTokenCreated command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Sent email notification");

        return Task.CompletedTask;
    }
}