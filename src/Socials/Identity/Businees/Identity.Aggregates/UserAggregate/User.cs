using TL.SharedKernel.Business.Aggregates;

namespace TL.Socials.Identity.Business.Aggregates.UserAggregate;

public sealed class User : AggregateRoot<Guid>
{
    public User(
        Guid id,
        Email email,
        string passwordHash,
        DateTimeOffset createdDate,
        DateTimeOffset updatedDate) : base(id)
    {
        ArgumentNullException.ThrowIfNull(email);
        ArgumentNullException.ThrowIfNull(passwordHash);

        Email = email;
        PasswordHash = passwordHash;
        CreatedDate = createdDate;
        UpdatedDate = updatedDate;
    }

    public static User Create(Email email, string password)
    {
        ArgumentNullException.ThrowIfNull(password);

        var now = DateTimeOffset.UtcNow;
        return new User(
            Guid.NewGuid(),
            email,
            BCrypt.Net.BCrypt.HashPassword(password),
            now,
            now);
    }

    public Email Email { get; }
    public string PasswordHash { get; }
    public DateTimeOffset CreatedDate { get; }
    public DateTimeOffset UpdatedDate { get; }
    public bool IsPasswordValid(string password) => BCrypt.Net.BCrypt.Verify(password, PasswordHash);
}