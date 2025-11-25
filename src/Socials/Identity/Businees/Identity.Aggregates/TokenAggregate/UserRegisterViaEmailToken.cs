using TL.Socials.Identity.Business.Aggregates.UserAggregate;

namespace TL.Socials.Identity.Business.Aggregates.TokenAggregate;

public sealed class UserRegisterViaEmailToken : Token
{
    public Email Email { get; }

    public UserRegisterViaEmailToken(
        Guid id,
        Email email,
        string value,
        DateTimeOffset createdAt,
        DateTimeOffset expiresAt)
        : base(id, value, createdAt, expiresAt)
    {
        ArgumentNullException.ThrowIfNull(email);

        Email = email;
    }

    public static UserRegisterViaEmailToken Create(Email email)
    {
        var createdAt = DateTimeOffset.UtcNow;
        var expiresAt = createdAt.Add(DefaultTokenLifetime);
        var token = new UserRegisterViaEmailToken(
            Guid.NewGuid(),
            email,
            Guid.NewGuid().ToString(),
            createdAt,
            expiresAt);
        token.Raise(new UserRegisterViaEmailTokenCreated(Guid.NewGuid(), token.Id));
        return token;
    }

    public bool IsValid(Email email)
        => Email.Equals(email) && DateTimeOffset.UtcNow <= ExpiresAt;

    public void UpdateTokenValue()
    {
        CreatedAt = DateTimeOffset.UtcNow;
        ExpiresAt = CreatedAt.Add(DefaultTokenLifetime);
        Value = Guid.NewGuid().ToString();
    }

    private static TimeSpan DefaultTokenLifetime => TimeSpan.FromMinutes(30);
}
