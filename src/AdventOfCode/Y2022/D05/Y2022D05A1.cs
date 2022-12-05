using AdventOfCode.Lib;
using AdventOfCode.Lib.Attributes;

namespace AdventOfCode.Y2022.D05;

/// <summary>
/// Alternative solution #1
/// Using strings as the storage medium rather than Stack.
/// It's slower anyway lol.
/// </summary>
public class Y2022D05A1 : ISolution
{
    [ExpectedResult("MQTPGLLDN", Sample = "CMZ")]
    public object SolvePartOne(string input)
    {
        var stacks = ParseStartStacks(input)
            .ToList();
        
        foreach ((int cnt, int frm, int to) in ParseMoves(input))
        {
            stacks[to] += new string(stacks[frm][(stacks[frm].Length - cnt)..].Reverse().ToArray());
            stacks[frm] = stacks[frm][..^cnt];
        }

        return string.Join("", stacks.Select(s => s.Last()));
    }

    [ExpectedResult("LVZPSTTCZ", Sample = "MCD")]
    public object SolvePartTwo(string input)
    {
        var stacks = ParseStartStacks(input)
            .ToList();
        
        foreach ((int cnt, int frm, int to) in ParseMoves(input))
        {
            // Just don't reverse
            stacks[to] += new string(stacks[frm][(stacks[frm].Length - cnt)..].ToArray());
            stacks[frm] = stacks[frm][..^cnt];
        }

        return string.Join("", stacks.Select(s => s.Last()));
    }

    private IEnumerable<string> ParseStartStacks(string input)
    {
        int count = input.IndexOf('\n') / 4 + 1;
        return InputHelpers.AsLines(input.Split("\n\n")[0])
                           .SkipLast(1)
                           .Select(l => l.Skip(1)
                                         .Where((c, idx) => idx % 4 == 0))
                           .SelectMany(c => c)
                           .Select((c, idx) => (c, idx))
                           .GroupBy(r => r.idx % count, r => r.c)
                           .Select(g => new string(g.Where(c => c != ' ').Reverse().ToArray()));
    }

    private IEnumerable<(int cnt, int frm, int to)> ParseMoves(string input)
    {
        return InputHelpers.AsLines(input.Split("\n\n")[1])
                           .Select(l => l.Split(' ')
                                         .Where((w, idx) => idx % 2 == 1)
                                         .ToArray())
                           .Select(arr => (int.Parse(arr[0]), int.Parse(arr[1]) - 1, int.Parse(arr[2]) - 1))
                           .ToArray();
    }
}