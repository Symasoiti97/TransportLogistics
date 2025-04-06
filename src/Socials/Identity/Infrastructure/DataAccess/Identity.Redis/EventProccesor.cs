using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using TL.SharedKernel.Application.Commands;
using TL.Socials.Identity.Infrastructure.DataAccess.Redis.Extensions;
using TL.Socials.Identity.Infrastructure.DataAccess.Redis.Options;
using IRedisDatabase = StackExchange.Redis.IDatabase;

namespace TL.Socials.Identity.Infrastructure.DataAccess.Redis;

public sealed class EventProcessor(
    IRedisDatabase database,
    EventProcessorOptions options,
    ILogger<EventProcessor> logger,
    IServiceProvider serviceProvider,
    JsonSerializerOptions serializerOptions)
{
    private const int MaxRetries = 5;

    public async Task ProcessAsync(CancellationToken cancellationToken)
    {
        try
        {
            database.StreamCreateConsumerGroup(options.StreamName, options.ConsumerGroup, "0-0", createStream: true);
        }
        catch (RedisServerException ex) when (ex.Message.Contains("BUSYGROUP"))
        {
            logger.LogDebug("Consumer group already exists");
        }

        while (!cancellationToken.IsCancellationRequested)
        {
            var messages = await database.StreamReadGroupAsync(
                options.StreamName,
                options.ConsumerGroup,
                options.ConsumerName,
                ">",
                count: 1);

            foreach (var message in messages)
            {
                var @event = message.GetEvent(serializerOptions);
                var retries = message.GetRetries();
                var handlerType = message.TryGetHandlerType();

                logger.LogInformation(
                    "Processing event {EventId} of {EventType} type. Retries: {Retries}",
                    @event.Id,
                    @event.GetType().Name,
                    handlerType);

                var eventHandlerType = typeof(IUseCaseHandler<>).MakeGenericType(@event.GetType());
                var handlers = (IEnumerable<IUseCaseHandler>) serviceProvider.GetServices(eventHandlerType);

                foreach (var handler in handlers)
                {
                    if (handlerType is not null && handler.GetType().Name != handlerType)
                    {
                        continue;
                    }

                    try
                    {
                        using var serviceScope = serviceProvider.CreateScope();
                        await handler.HandleAsync(@event, cancellationToken);
                    }
                    catch (Exception exception)
                    {
                        logger.LogError(exception, "Error processing event {MessageId}", message.Id);

                        if (retries < MaxRetries)
                        {
                            var newRetries = retries + 1;
                            database.StreamAdd(
                                options.StreamName,
                                [
                                    new NameValueEntry("event", JsonSerializer.Serialize(@event)),
                                    new NameValueEntry("retries", newRetries),
                                    new NameValueEntry("handler-type", handler.GetType().Name)
                                ]);

                            logger.LogInformation(
                                "Retrying event {MessageId} (Attempt {NewRetries}/{MaxRetries})",
                                message.Id,
                                newRetries,
                                MaxRetries);
                        }
                        else
                        {
                            database.StreamAdd(
                                options.DlqStreamName,
                                [
                                    new NameValueEntry("event", JsonSerializer.Serialize(@event)),
                                    new NameValueEntry("retries", retries),
                                    new NameValueEntry("handler-type", handler.GetType().Name)
                                ]);

                            logger.LogInformation("Moving event {MessageId} to Dead-Letter Queue", message.Id);
                        }
                    }
                }

                database.StreamAcknowledge(options.StreamName, options.ConsumerGroup, message.Id);
                logger.LogInformation("Event {MessageId} processed", message.Id);
            }

            await Task.Delay(millisecondsDelay: 5000, cancellationToken);
        }
    }
}