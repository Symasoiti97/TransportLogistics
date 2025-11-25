namespace TL.Socials.Identity.Infrastructure.DataAccess.Redis.Options;

public sealed class EventProcessorOptions
{
    public string StreamName { get; }
    public string ConsumerGroup { get; }
    public string ConsumerName { get; }
    public string DlqStreamName { get; }

    public EventProcessorOptions(string streamName, string consumerGroup, string consumerName, string dlqStreamName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(streamName);
        ArgumentException.ThrowIfNullOrWhiteSpace(consumerGroup);
        ArgumentException.ThrowIfNullOrWhiteSpace(consumerName);
        ArgumentException.ThrowIfNullOrWhiteSpace(dlqStreamName);

        StreamName = streamName;
        ConsumerGroup = consumerGroup;
        ConsumerName = consumerName;
        DlqStreamName = dlqStreamName;
    }
}
