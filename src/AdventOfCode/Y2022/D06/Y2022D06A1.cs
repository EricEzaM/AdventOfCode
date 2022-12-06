using AdventOfCode.Lib;
using AdventOfCode.Lib.Attributes;

namespace AdventOfCode.Y2022.D06;

/// <summary>
/// Alternative solution #1
/// No spans.
/// Code is much simpler - but it is slower and uses more memory. Example run:
/// ┌───────┬─────┬─────┬──────┬───────────┬─────────────────┬──────┬───────────┬─────────────────┐
/// │ Year  │ Day │ Alt │ P1   │ P1 time   │ P1 mem kb delta │ P2   │ P2 time   │ P2 mem kb delta │
/// ├───────┼─────┼─────┼──────┼───────────┼─────────────────┼──────┼───────────┼─────────────────┤
/// │ Y2022 │ D06 │ 0   │ 1538 │ 0.2993 ms │ 0               │ 2315 │ 0.1776 ms │ 0               │
/// │ Y2022 │ D06 │ 1   │ 1538 │ 0.9476 ms │ 444.812         │ 2315 │ 1.5048 ms │ 1480            │
/// └───────┴─────┴─────┴──────┴───────────┴─────────────────┴──────┴───────────┴─────────────────┘
/// </summary>
public class Y2022D06A1 : ISolution
{
    [ExpectedResult(1538, Sample = 7)]
    public object SolvePartOne(string input)
    {
        return FindStart(input, 4);
    }

    [ExpectedResult(2315, Sample = 19)]
    public object SolvePartTwo(string input)
    {
        return FindStart(input, 14);
    }

    private int FindStart(string input, int requiredUniqueChars)
    {
        for (int i = 0; i < input.Length; i++)
        {
            if (input.Substring(i, requiredUniqueChars).Distinct().Count() == requiredUniqueChars)
            {
                return i + requiredUniqueChars;
            }
        }

        return 0;
    }
}