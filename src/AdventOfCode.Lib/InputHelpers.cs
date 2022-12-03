namespace AdventOfCode.Lib;

public static class InputHelpers
{
    public static IEnumerable<string> AsLines(string input) =>
        input.Split('\n');
    
    public static IEnumerable<int> AsInts(string input) =>
        AsLines(input)
            .Select(int.Parse);
}