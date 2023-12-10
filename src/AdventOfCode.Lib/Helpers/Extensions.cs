using System.Globalization;

namespace AdventOfCode.Lib.Helpers;

public static class Extensions
{
    public static string[] AsLines(this string input) => input.Split('\n');

    public static IEnumerable<T> ParseAs<T>(this IEnumerable<string> input) where T : IParsable<T> => input.Select(s =>  T.Parse(s, CultureInfo.InvariantCulture));
}