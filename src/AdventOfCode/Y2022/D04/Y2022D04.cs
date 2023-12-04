using AdventOfCode.Lib;
using AdventOfCode.Lib.Attributes;
using AdventOfCode.Lib.Helpers;

namespace AdventOfCode.Y2022.D04;

public class Y2022D04 : ISolution
{
    [ExpectedResult(483, Sample = 2)]
    public object SolvePartOne(string input)
    {
        return InputHelpers
               .AsLines(input)
               // Parse each row into one 4-element int array
               .Select(l => l.Split(',')
                             .SelectMany(pair => pair.Split('-'))
                             .Select(int.Parse)
                             .ToArray())
               // This select could be omitted - it's just for readability
               .Select(values => new
               {
                   E1Min = values[0],
                   E1Max = values[1],
                   E2Min = values[2],
                   E2Max = values[3],
               })
               // Check if one elf fully encompasses the other
               .Count(r => (r.E1Min <= r.E2Min && r.E1Max >= r.E2Max) ||
                           (r.E2Min <= r.E1Min && r.E2Max >= r.E1Max));
    }

    [ExpectedResult(874, Sample = 4)]
    public object SolvePartTwo(string input)
    {
        return InputHelpers
               .AsLines(input)
               // Parse each row into one 4-element int array
               .Select(l => l.Split(',')
                             .SelectMany(pair => pair.Split('-'))
                             .Select(int.Parse)
                             .ToArray())
               // This select could be omitted - it's just for readability
               .Select(values => new
               {
                   E1Min = values[0],
                   E1Max = values[1],
                   E2Min = values[2],
                   E2Max = values[3],
               })
               // Check if there is intersect
               .Count(r => r.E1Max >= r.E2Min && r.E2Max >= r.E1Min);
    }
}