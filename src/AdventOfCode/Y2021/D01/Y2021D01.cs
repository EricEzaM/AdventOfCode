using AdventOfCode.Lib;
using AdventOfCode.Lib.Helpers;

namespace AdventOfCode.Y2021.D01;

public class Y2021D01 : ISolution
{
    public object SolvePartOne(string input)
    {
        var numbers = InputHelpers.AsInts(input);
        return GetIncreaseCount(numbers).ToString();
    }

    public object SolvePartTwo(string input)
    {
        var numbers = InputHelpers.AsInts(input);
        return GetIncreaseCount(GetThreeElementSlidingWindowSums(numbers)).ToString();
    }

    private IEnumerable<int> GetThreeElementSlidingWindowSums(IEnumerable<int> numbers)
    {
        var list = numbers.ToList();
        return list.Select((_, idx) => list.Skip(idx).Take(3).Sum())
                      .ToList();
    }

    private int GetIncreaseCount(IEnumerable<int> numbers) =>
        Enumerable.Zip(numbers, numbers.Skip(1))
                  .Count(t => t.Second > t.First);
}