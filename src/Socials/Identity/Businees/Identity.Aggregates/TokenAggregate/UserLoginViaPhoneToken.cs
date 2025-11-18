using System.Security.Cryptography;
using TL.Socials.Identity.Business.Aggregates.UserAggregate;

namespace TL.Socials.Identity.Business.Aggregates.TokenAggregate;

public sealed class UserLoginViaPhoneToken : Token
{
    private const int VerificationCodeLength = 6;

    public PhoneNumber PhoneNumber { get; }

    public UserLoginViaPhoneToken(
        Guid id,
        PhoneNumber phoneNumber,
        string value,
        DateTimeOffset createdAt,
        DateTimeOffset expiresAt)
        : base(
            id,
            value,
            createdAt,
            expiresAt)
    {
        ArgumentNullException.ThrowIfNull(phoneNumber);

        PhoneNumber = phoneNumber;
    }

    public static UserLoginViaPhoneToken Create(PhoneNumber phoneNumber)
    {
        var createdAt = DateTimeOffset.UtcNow;
        var expiresAt = createdAt.Add(DefaultTokenLifetime);
        var token = new UserLoginViaPhoneToken(
            Guid.NewGuid(),
            phoneNumber,
            GenerateVerificationCode(),
            createdAt,
            expiresAt);
        token.Raise(new UserLoginViaPhoneTokenCreated(Guid.NewGuid(), token.Id));
        return token;
    }

    public void UpdateTokenValue()
    {
        CreatedAt = DateTimeOffset.UtcNow;
        ExpiresAt = CreatedAt.Add(DefaultTokenLifetime);
        Value = GenerateVerificationCode();
    }

    public bool IsValid(PhoneNumber phoneNumber)
        => PhoneNumber.Equals(phoneNumber) && DateTimeOffset.UtcNow <= ExpiresAt;

    private static string GenerateVerificationCode()
    {
        var min = (int) Math.Pow(10, VerificationCodeLength - 1);
        var max = (int) Math.Pow(10, VerificationCodeLength);

        return RandomNumberGenerator.GetInt32(min, max).ToString();
    }

    private static TimeSpan DefaultTokenLifetime => TimeSpan.FromMinutes(10);
}