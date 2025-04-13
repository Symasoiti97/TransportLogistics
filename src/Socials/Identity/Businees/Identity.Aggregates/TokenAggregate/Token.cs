using TL.SharedKernel.Business.Aggregates;

namespace TL.Socials.Identity.Business.Aggregates.TokenAggregate;

public abstract class Token : AggregateRoot<Guid>
{
    protected Token(Guid id, string value, DateTimeOffset createdAt, DateTimeOffset expiresAt) : base(id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        Value = value;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
    }

    public string Value { get; protected set; }
    public DateTimeOffset CreatedAt { get; protected set; }
    public DateTimeOffset ExpiresAt { get; protected set; }
}