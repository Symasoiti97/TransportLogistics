using TL.SharedKernel.Business.Aggregates;

namespace TL.Socials.Identity.Business.Aggregates.TokenAggregate;

public abstract class Token : AggregateRoot<Guid>
{
    protected Token(Guid id, string value) : base(id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        Value = value;
        CreatedAt = DateTimeOffset.UtcNow;
        ExpiresAt = CreatedAt.Add(DefaultTokenLifetime);
    }

    private static TimeSpan DefaultTokenLifetime => TimeSpan.FromMinutes(5);

    public string Value { get; private set; }
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset ExpiresAt { get; }

    public void UpdateTokenValue()
    {
        Value = GenerateTokenValue();
    }

    protected abstract string GenerateTokenValue();
}