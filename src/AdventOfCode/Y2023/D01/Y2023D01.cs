using System.CodeDom.Compiler;
using AdventOfCode.Lib;
using AdventOfCode.Lib.Attributes;
using AdventOfCode.Lib.Helpers;

namespace AdventOfCode.Y2023.D01;

public class Y2023D01 : ISolution
{
    [ExpectedResult(55130, Sample = 142)]
    public object SolvePartOne(string input)
    {
        return InputHelpers.AsLines(input)
            .Select(c => c.ToCharArray().Where(char.IsDigit))
            .Select(nums => nums.ToList())
            .Sum(digitsOnLine => int.Parse(digitsOnLine.First().ToString() + digitsOnLine.Last()));
    }
    
    /*
     * There are certainly smarter solutions for this which I saw after completing it.
     * - Using Right-to-left regular expressions.
     * - Using .Replace to replace with a digit but maintaining the start/end characters, e.g. one => o1e, two => t2o, three => t3e, etc
     */
    [ExpectedResult(54985, Sample = 281)]
    public object SolvePartTwo(string input)
    {
        return InputHelpers.AsLines(input)
            .Select(GetNumbersFromLine)
            .Select(nums => nums.ToList())
            .Sum(numbersOnLine => int.Parse(numbersOnLine.First().ToString() + numbersOnLine.Last()));
    }

    private IEnumerable<int> GetNumbersFromLine(string line)
    {
        List<int> digits = new();

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            if (char.IsDigit(c))
            {
                digits.Add((int)char.GetNumericValue(c));
                continue;
            }

            // Get the remaining string to test for the number-as-a-word pattern
            string s = line.Substring(i);
            for (int j = 0; j < s.Length; j++)
            {
                // If the character we are up to is a digit, it can't be a spelled out number any more, so break.
                if (char.IsDigit(s[j]))
                {
                    break;
                }

                // Test every possible string for the number. Overlaps are allowed, e.g. sevenine is both 7 AND 9.
                string testString = s.Substring(0, j + 1);
                int digit = testString switch
                {
                    "one" => 1,
                    "two" => 2,
                    "three" => 3,
                    "four" => 4,
                    "five" => 5,
                    "six" => 6,
                    "seven" => 7,
                    "eight" => 8,
                    "nine" => 9,
                    _ => 0
                };

                if (digit > 0)
                {
                    digits.Add(digit);
                }
            }
        }

        return digits;
    }
}