using TL.SharedKernel.Business.Aggregates;

namespace TL.Socials.Identity.Business.Aggregates.UserAggregate;

public sealed class User : AggregateRoot<Guid>
{
    public Email? Email { get; }
    public string? PasswordHash { get; }
    public PhoneNumber? PhoneNumber { get; }
    public DateTimeOffset CreatedDate { get; }
    public DateTimeOffset UpdatedDate { get; }

    public User(
        Guid id,
        Email? email,
        string? passwordHash,
        PhoneNumber? phoneNumber,
        DateTimeOffset createdDate,
        DateTimeOffset updatedDate) : base(id)
    {
        if (email is null && phoneNumber is null)
        {
            throw new Conflict().WithDetails("Email or phone number are required.");
        }

        if (email is not null && passwordHash is null)
        {
            throw new Conflict().WithDetails("Email and phone number are required.");
        }

        Email = email;
        PasswordHash = passwordHash;
        PhoneNumber = phoneNumber;
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
            phoneNumber: null,
            now,
            now);
    }

    public static User Create(PhoneNumber phoneNumber)
    {
        var now = DateTimeOffset.UtcNow;
        return new User(
            Guid.NewGuid(),
            email: null,
            passwordHash: null,
            phoneNumber,
            now,
            now);
    }

    public bool IsPasswordValid(string password) => BCrypt.Net.BCrypt.Verify(password, PasswordHash);
}