using AdventOfCode.Lib;

namespace AdventOfCode.Y2022.D01;

public class Y2022D01 : ISolution
{
    public object SolvePartOne(string input)
    {
        return GetGroupCalories(input).Max();
    }

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