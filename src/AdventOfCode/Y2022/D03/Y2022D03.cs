using AdventOfCode.Lib;
using AdventOfCode.Lib.Attributes;
using AdventOfCode.Lib.Helpers;

namespace AdventOfCode.Y2022.D03;

public class Y2022D03 : ISolution
{
    [ExpectedResult(8394, Sample = 157)]
    public object SolvePartOne(string input)
    {
        return InputHelpers.AsLines(input)
            .SelectMany(GetItemInBothCompartments)
            .Sum(CharPriority);
    }
    
    [ExpectedResult(2413, Sample = 70)]
    public object SolvePartTwo(string input)
    {
        return InputHelpers.AsLines(input)
            .Chunk(3)
            .Select(GetCommonItem)
            .Sum(CharPriority);
    }
    
    private int CharPriority(char c) => c switch
    {
        >= 'a' and <= 'z' => c - 'a' + 1,
        >= 'A' and <= 'Z' => c - 'A' + 27,
        _ => throw new ArgumentOutOfRangeException(nameof(c), c, null)
    };
    
    private char GetCommonItem(string[] rucksacks)
    {
        return rucksacks
            .Skip(1)
            .Aggregate(rucksacks.First().AsEnumerable(), (common, content) => common.Intersect(content))
            .First();
    }

    private IEnumerable<char> GetItemInBothCompartments(string contents) =>
        contents.Substring(0, contents.Length / 2)
            .Intersect(contents.Substring(contents.Length / 2));
}