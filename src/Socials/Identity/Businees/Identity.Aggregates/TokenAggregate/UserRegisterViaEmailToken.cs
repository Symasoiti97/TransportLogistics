using TL.Socials.Identity.Business.Aggregates.UserAggregate;

namespace TL.Socials.Identity.Business.Aggregates.TokenAggregate;

public sealed class UserRegisterViaEmailToken : Token
{
    public UserRegisterViaEmailToken(Guid id, Email email, string value) : base(id, value)
    {
        ArgumentNullException.ThrowIfNull(email);

        Email = email;
    }

    public Email Email { get; }

    public static UserRegisterViaEmailToken Create(Email email)
    {
        var token = new UserRegisterViaEmailToken(Guid.NewGuid(), email, Guid.NewGuid().ToString());
        token.Raise(new UserRegisterViaEmailTokenCreated(Guid.NewGuid(), token.Id));
        return token;
    }

    public bool IsValid(Email email)
        => Email.Equals(email) && DateTimeOffset.UtcNow <= ExpiresAt;

    protected override string GenerateTokenValue() => Guid.NewGuid().ToString();
}