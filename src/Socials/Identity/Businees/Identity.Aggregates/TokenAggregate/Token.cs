using TL.SharedKernel.Business.Aggregates;

namespace TL.Socials.Identity.Business.Aggregates.TokenAggregate;

public abstract class Token : AggregateRoot<Guid>
{
    protected Token(Guid id, string value) : base(id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        Value = value;
    }

    public string Value { get; private set; }

    public void UpdateTokenValue()
    {
        Value = GenerateTokenValue();
    }

    protected abstract string GenerateTokenValue();
}