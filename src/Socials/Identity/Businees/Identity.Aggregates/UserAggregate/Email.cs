using System.Text.RegularExpressions;
using TL.SharedKernel.Business.Aggregates;

namespace TL.Socials.Identity.Business.Aggregates.UserAggregate;

public sealed partial record Email
{
    public string Value { get; }

    public Email(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var normalized = value.Trim().ToLower();

        if (!EmailRegex().IsMatch(normalized))
        {
            throw new Conflict().WithDetails("Email is not valid.");
        }

        Value = normalized;
    }

    [GeneratedRegex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")]
    private static partial Regex EmailRegex();
}
