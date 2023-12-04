namespace AdventOfCode.Lib.Helpers;

public static class InputHelpers
{
    public static string[] AsLines(string input) =>
        input.Split('\n');

    public static char[][] AsCharGrid(string input) =>
        input.Split('\n')
            .Select(l => l.ToCharArray())
            .ToArray();

    public static Grid AsGrid(string input) =>
        new(input.Split('\n')
            .Select(l => l.ToCharArray())
            .ToArray());

    public static IEnumerable<int> AsInts(string input) =>
        AsLines(input)
            .Select(int.Parse);
}