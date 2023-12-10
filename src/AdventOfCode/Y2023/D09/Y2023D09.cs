using AdventOfCode.Lib;
using AdventOfCode.Lib.Attributes;
using AdventOfCode.Lib.Helpers;

namespace AdventOfCode.Y2023.D09;

public class Y2023D09 : ISolution
{
    [ExpectedResult(1789635132, Sample = 114)]
    public object SolvePartOne(string input)
    {
        var x = input.AsLines()
            .Select(s => s.Split(' ').ParseAs<int>().ToList())
            .ToList();

        return x.Select(CalculateDiffSequence)
            .Sum(s => s.Sum(e => e.Last()));
    }

    [ExpectedResult(913, Sample = 2)]
    public object SolvePartTwo(string input)
    {
        var x = input.AsLines()
            .Select(s => s.Split(' ').ParseAs<int>().Reverse().ToList())
            .ToList();

        return x.Select(CalculateDiffSequence)
            .Sum(s => s.Sum(e => e.Last()));
    }
    
    private List<List<int>> CalculateDiffSequence(List<int> list)
    {
        List<List<int>> diffSequence = new()
        {
            list
        };
        
        while (diffSequence.LastOrDefault()?.Any(i => i != 0) ?? true)
        {
            diffSequence.Add(CalculateDifference(diffSequence.LastOrDefault() ?? list));
        }

        return diffSequence;
    }

    private List<int> CalculateDifference(List<int> list)
    {
        var ret = new List<int>(list.Count - 1);

        for (int i = 0; i < list.Count - 1; i++)
        {
            ret.Add(list[i + 1] - list[i]);
        }

        return ret;
    }
}