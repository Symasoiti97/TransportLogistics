using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace TL.Socials.Identity.Business.Aggregates.UserAggregate;

public sealed partial record Email : IParsable<Email>
{
    public string Value { get; }

    private Email(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        var normalized = ParseAndNormalize(value);
        if (normalized != value)
        {
            throw new ArgumentException("Email must be normalized (trimmed and lowercase).", nameof(value));
        }

        Value = value;
    }

    public static Email Parse(string source, IFormatProvider? provider = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(source);

        return new Email(ParseAndNormalize(source));
    }

    public static bool TryParse([NotNullWhen(true)] string? source,
                                IFormatProvider? provider,
                                [MaybeNullWhen(false)] out Email result)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            result = null;
            return false;
        }

        try
        {
            result = Parse(source, provider);
            return true;
        }
        catch (ArgumentException)
        {
            result = null;
            return false;
        }
    }

    private static string ParseAndNormalize(string source)
    {
        var normalized = source.Trim().ToLower();

        return EmailRegex().IsMatch(normalized)
            ? normalized
            : throw new ArgumentException("Invalid email format", nameof(source));
    }

    [GeneratedRegex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")]
    private static partial Regex EmailRegex();
}
