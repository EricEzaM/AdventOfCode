namespace AdventOfCode.Lib;

public static class InputHelpers
{
    public static IEnumerable<string> AsLines(string input) =>
        input.Split('\n');
    
    public static char[][] AsGrid(string input) =>
        input.Split('\n')
             .Select(l => l.ToCharArray())
             .ToArray();
    
    public static IEnumerable<int> AsInts(string input) =>
        AsLines(input)
            .Select(int.Parse);
}