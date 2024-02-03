using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Text.Unicode;
using Microsoft.EntityFrameworkCore;

namespace TL.Locations.Locations.SyncLocationsTool.Infrastructure.Services;

internal static class LocationExtensions
{
    private static IEnumerable<string> LocationNames { get; } = CultureInfo.GetCultures(CultureTypes.AllCultures)
        .Select(x => "name:" + x.TwoLetterISOLanguageName).Append("name").Distinct().ToArray();

    public static IEnumerable<KeyValuePair<string, string>> FilterNamesByCultures(
        this IEnumerable<KeyValuePair<string, string>> source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        return source.Where(i => LocationNames.Contains(i.Key));
    }

    public static IEnumerable<KeyValuePair<string, string>> JoinNames(
        this IEnumerable<KeyValuePair<string, string>> source,
        IEnumerable<KeyValuePair<string, string>> names)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (names == null) throw new ArgumentNullException(nameof(names));

        return source.Join(
            names,
            pair => pair.Key,
            pair => pair.Key,
            (pair, valuePair) =>
                new KeyValuePair<string, string>(pair.Key, pair.Value + ", " + valuePair.Value));
    }

    public static string? BuildFullTxt(this IEnumerable<KeyValuePair<string, string>> source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));

        return source.Any() ? string.Join(" | ", source.Select(x => x.Value).Distinct()) : null;
    }

    public static async Task<IEnumerable<T>> RawSqlQuery<T>(
        this DbContext context,
        string query,
        Func<DbDataReader, T> map)
    {
        await using var command = context.Database.GetDbConnection().CreateCommand();
        command.CommandText = query;
        command.CommandType = CommandType.Text;

        await context.Database.OpenConnectionAsync();

        await using var result = await command.ExecuteReaderAsync();
        var entities = new List<T>();

        while (await result.ReadAsync())
        {
            entities.Add(map(result));
        }

        return entities;
    }

    public static bool IsCyrillic(this string value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));

        return value.Any(
            c => c >= UnicodeRanges.Cyrillic.FirstCodePoint &&
                 c < UnicodeRanges.Cyrillic.FirstCodePoint + UnicodeRanges.Cyrillic.Length);
    }

    public static bool IsCyrillicFirstSymbol(this string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(value));

        var c = value.First();
        return c >= UnicodeRanges.Cyrillic.FirstCodePoint &&
               c < UnicodeRanges.Cyrillic.FirstCodePoint + UnicodeRanges.Cyrillic.Length;
    }

    public static bool IsBasicLatinFirstSymbol(this string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(value));

        var c = value.First();
        return c >= UnicodeRanges.BasicLatin.FirstCodePoint &&
               c < UnicodeRanges.BasicLatin.FirstCodePoint + UnicodeRanges.BasicLatin.Length;
    }
}