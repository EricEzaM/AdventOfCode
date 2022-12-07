using AdventOfCode.Lib;
using AdventOfCode.Lib.Attributes;

namespace AdventOfCode.Y2022.D07;

/// <summary>
/// Alternate Solution #1
/// Fancy switch :o
/// Appears to be faster but uses more memory (string/other allocations in the switch I assume?)
/// </summary>
public class Y2022D07A1 : ISolution
{
    [ExpectedResult(1297159, Sample = 95437)]
    public object SolvePartOne(string input)
    {
        return CalcDirSizes(input)
               .Where(kvp => kvp.Value < 100000)
               .Sum(kvp => kvp.Value);
    }

    [ExpectedResult(3866390, Sample = 24933642)]
    public object SolvePartTwo(string input)
    {
        Dictionary<string, int> dirSizes = CalcDirSizes(input);
        int requiredSpace = 30000000 - (70000000 - dirSizes["/"]);
        return dirSizes.OrderBy(kvp => kvp.Value)
                       .First(i => i.Value > requiredSpace)
                       .Value;
    }

    private Dictionary<string, int> CalcDirSizes(string input)
    {
        Dictionary<string, int> dirSizes = new();
        Stack<string> currDir = new();

        foreach (string line in InputHelpers.AsLines(input))
        {
            // Switch returns an action which is immediately called.
            (line.Split(' ') switch
            {
                ["$", "cd", "", ..] => new Action(() => currDir.Clear()),
                ["$", "cd", ".."] => () => currDir.Pop(),
                ["$", "cd", var dir] => () => currDir.Push(dir),
                ["$", "ls"] => () => { },
                ["dir", ..] => () => { },
                [var fSize, _] => () =>
                {
                    int fSizeInt = int.Parse(fSize);
                    for (int i = 0; i < currDir.Count; i++)
                    {
                        string dir = string.Join("_", currDir.Reverse().Take(i + 1));
                        dirSizes[dir] = !dirSizes.ContainsKey(dir) ? fSizeInt : dirSizes[dir] + fSizeInt;
                    }
                },
                _ => () => { }
            })();
        }

        return dirSizes;
    }
}