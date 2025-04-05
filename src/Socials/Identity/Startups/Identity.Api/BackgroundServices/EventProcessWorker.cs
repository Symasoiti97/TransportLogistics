using TL.Socials.Identity.Infrastructure.DataAccess.Redis;

namespace TL.Socials.Identity.Startups.Api.BackgroundServices;

public sealed class EventProcessWorker(EventProcessor eventProcessor) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await eventProcessor.ProcessAsync(cancellationToken);
    }
}