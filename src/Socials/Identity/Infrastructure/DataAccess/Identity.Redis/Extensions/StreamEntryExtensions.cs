using System.Text.Json;
using StackExchange.Redis;
using TL.SharedKernel.Business.Aggregates;

namespace TL.Socials.Identity.Infrastructure.DataAccess.Redis.Extensions;

internal static class StreamEntryExtensions
{
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

        return (int) entry.Value;
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