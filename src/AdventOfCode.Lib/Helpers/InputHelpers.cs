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

    public static Grid<Cell<T>, T> AsGrid<T>(string input, Func<char, T> transform) where T : struct =>
        new Grid<Cell<T>, T>(input.Split('\n')
            .Select(l => l.Select(transform).ToArray())
            .ToArray());

    public static IEnumerable<int> AsInts(string input) =>
        AsLines(input)
            .Select(int.Parse);
}