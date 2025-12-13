using System.Diagnostics.CodeAnalysis;
using PhoneNumbers;

namespace TL.Socials.Identity.Business.Aggregates.UserAggregate;

public sealed record PhoneNumber : IParsable<PhoneNumber>
{
    public string Value { get; }

    private PhoneNumber(string value)
    {
        var parsedNumber = ParseAndFormat(value);
        if (parsedNumber != value)
        {
            throw new ArgumentException("Phone number must be in E164 format.", nameof(value));
        }

        Value = value;
    }

    public static PhoneNumber Parse(string source, IFormatProvider? provider = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(source);

        return new PhoneNumber(ParseAndFormat(source));
    }

    public static bool TryParse([NotNullWhen(true)] string? source,
                                IFormatProvider? provider,
                                [MaybeNullWhen(false)] out PhoneNumber result)
    {
        if (source is null)
        {
            result = null;
            return false;
        }

        try
        {
            result = Parse(source, provider);
            return true;
        }
        catch (Exception)
        {
            result = null;
            return false;
        }
    }

    private static string ParseAndFormat(string source)
    {
        var phoneNumberUtil = PhoneNumberUtil.GetInstance();
        var phoneNumber = phoneNumberUtil.Parse(source, defaultRegion: null);

        if (!phoneNumberUtil.IsValidNumber(phoneNumber))
        {
            throw new ArgumentException("Invalid phone number", nameof(source));
        }

        return phoneNumberUtil.Format(phoneNumber, PhoneNumberFormat.E164);
    }
}
