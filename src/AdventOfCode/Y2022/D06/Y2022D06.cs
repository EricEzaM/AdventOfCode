using AdventOfCode.Lib;
using AdventOfCode.Lib.Attributes;

namespace AdventOfCode.Y2022.D06;

/// <summary>
/// Using Spans
/// </summary>
public class Y2022D06 : ISolution
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
        int startsAt = 0;
        ReadOnlySpan<char> span = input.AsSpan();
        for (int i = 0; i < span.Length - requiredUniqueChars; i++)
        {
            bool unique = true;
            var slice = span.Slice(i, requiredUniqueChars);
            for (int j = 0; j < slice.Length; j++)
            {
                bool dupe = slice.LastIndexOf(slice[j]) != j;
                if (dupe)
                {
                    unique = false;
                    break;
                }
            }

            if (unique)
            {
                startsAt = i + requiredUniqueChars;
                break;
            }
        }

        return startsAt;
    }
}