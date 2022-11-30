namespace AdventOfCode.Lib;

public static class InputHelpers
{
    public static IEnumerable<int> AsInts(string input)
    {
        return input
               .Split('\n')
               .Select(int.Parse);
    }
}