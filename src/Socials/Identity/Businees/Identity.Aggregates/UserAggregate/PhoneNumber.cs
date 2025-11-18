using PhoneNumbers;
using TL.SharedKernel.Business.Aggregates;

namespace TL.Socials.Identity.Business.Aggregates.UserAggregate;

public sealed class PhoneNumber : ValueObject
{
    public string Value { get; }

    public PhoneNumber(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var phoneNumberUtil = PhoneNumberUtil.GetInstance();
        var phoneNumber = phoneNumberUtil.Parse(value, defaultRegion: null);

        Value = phoneNumberUtil.Format(phoneNumber, PhoneNumberFormat.E164);
    }

    protected override IEnumerable<object> GetEqualityComponents() => [Value];
}