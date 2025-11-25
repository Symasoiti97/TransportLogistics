using System.Text.Json;
using StackExchange.Redis;
using TL.SharedKernel.Business.Aggregates;
using TL.Socials.Identity.Infrastructure.DataAccess.Redis.Options;

namespace TL.Socials.Identity.Infrastructure.DataAccess.Redis.Extensions;

internal static class RedisExtensions
{
    public static void AddEventMessages(
        this IDatabaseAsync database,
        IEnumerable<Event> events,
        EventProcessorOptions eventOptions,
        JsonSerializerOptions serializerOptions)
    {
        foreach (var @event in events)
        {
            database.AddEventMessage(@event, retries: null, handlerType: null, eventOptions, serializerOptions);
        }
    }

    public static void AddEventMessage(
        this IDatabaseAsync database,
        Event @event,
        int? retries,
        Type? handlerType,
        EventProcessorOptions eventOptions,
        JsonSerializerOptions serializerOptions)
    {
        var entries = new List<NameValueEntry> { new("event", JsonSerializer.Serialize(@event, serializerOptions)) };

        if (retries is not null)
        {
            entries.Add(new NameValueEntry("retries", retries));
        }

        if (handlerType is not null)
        {
            entries.Add(new NameValueEntry("handler-type", handlerType.Name));
        }

        _ = database.StreamAddAsync(eventOptions.StreamName, entries.ToArray());
    }

    public static Event GetEvent(this StreamEntry eventMessage, JsonSerializerOptions serializerOptions)
    {
        string eventJson = eventMessage.Values.Single(v => v.Name == "event").Value!;
        return JsonSerializer.Deserialize<Event>(eventJson, serializerOptions)
               ?? throw new InvalidOperationException("Invalid event");
    }

    public static int GetRetries(this StreamEntry eventMessage)
    {
        var entry = eventMessage.Values.SingleOrDefault(value => value.Name == "retries");

        if (!entry.Value.HasValue || !entry.Value.IsInteger)
        {
            return 0;
        }

        return (int)entry.Value;
    }

    public static string? TryGetHandlerType(this StreamEntry eventMessage)
    {
        var entry = eventMessage.Values.SingleOrDefault(value => value.Name == "handler-type");

        if (!entry.Value.HasValue)
        {
            return null;
        }

        return entry.Value;
    }
}
