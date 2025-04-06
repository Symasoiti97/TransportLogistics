using TL.SharedKernel.Business.Aggregates;
using TL.Socials.Identity.Business.Aggregates.UserAggregate;

namespace TL.Socials.Identity.Business.Aggregates.TokenAggregate;

public sealed class UserRegisterViaEmailToken : AggregateRoot<Guid>
{
    public UserRegisterViaEmailToken(Guid id, Email email, string tokenValue) : base(id)
    {
        ArgumentNullException.ThrowIfNull(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(tokenValue);

        Email = email;
        Value = tokenValue;
    }

    public Email Email { get; }
    public string Value { get; }

    public static UserRegisterViaEmailToken Create(Email email)
    {
        var token = new UserRegisterViaEmailToken(Guid.NewGuid(), email, Guid.NewGuid().ToString());
        token.Raise(new UserRegisterViaEmailTokenCreated(Guid.NewGuid(), token.Id));
        return token;
    }
}