using AdventOfCode.Lib;
using AdventOfCode.Lib.Attributes;

namespace AdventOfCode.Y2022.D01;

public class Y2022D01 : ISolution
{
    [ExpectedResult(68467, Sample = 24000)]
    public object SolvePartOne(string input)
    {
        return GetGroupCalories(input).Max();
    }

    [ExpectedResult(203420, Sample = 45000)]
    public object SolvePartTwo(string input)
    {
        return GetGroupCalories(input)
               .OrderDescending()
               .Take(3)
               .Sum();
    }

    private IEnumerable<int> GetGroupCalories(string input) =>
        input.Split("\n\n")
             .Select(snackGroup => snackGroup.Split('\n').Sum(int.Parse));
}