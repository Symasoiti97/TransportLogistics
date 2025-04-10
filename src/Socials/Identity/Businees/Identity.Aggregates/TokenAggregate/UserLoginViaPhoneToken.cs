using TL.Socials.Identity.Business.Aggregates.UserAggregate;

namespace TL.Socials.Identity.Business.Aggregates.TokenAggregate;

public sealed class UserLoginViaPhoneToken : Token
{
    private const int VerificationCodeLength = 6;
    private static readonly Random Random = new();

    public UserLoginViaPhoneToken(Guid id, PhoneNumber phoneNumber, string value) : base(id, value)
    {
        ArgumentNullException.ThrowIfNull(phoneNumber);

        PhoneNumber = phoneNumber;
    }

    public PhoneNumber PhoneNumber { get; }

    public static UserLoginViaPhoneToken Create(PhoneNumber phoneNumber)
    {
        var token = new UserLoginViaPhoneToken(Guid.NewGuid(), phoneNumber, GenerateVerificationCode());
        token.Raise(new UserLoginViaPhoneTokenCreated(Guid.NewGuid(), token.Id));
        return token;
    }

    private static string GenerateVerificationCode()
        => string.Join(
            "",
            Enumerable.Range(start: 0, VerificationCodeLength)
                .Select(_ => Random.Next(minValue: 0, maxValue: 10)));

    protected override string GenerateTokenValue() => GenerateVerificationCode();
}