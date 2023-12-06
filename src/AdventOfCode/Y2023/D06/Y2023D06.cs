using AdventOfCode.Lib;
using AdventOfCode.Lib.Attributes;
using AdventOfCode.Lib.Helpers;

namespace AdventOfCode.Y2023.D06;

public class Y2023D06 : ISolution
{
    private record Race(long Time, long Distance);

    [ExpectedResult(393120, Sample = 288)]
    public object SolvePartOne(string input)
    {
        var lines = InputHelpers.AsLines(input);

        List<Race> races = GetIntegers(lines[0])
            .Zip(GetIntegers(lines[1]))
            .Select(t => new Race(t.First, t.Second))
            .ToList();

        return CalculateNumberOfWaysToWin(races);
    }

    [ExpectedResult(36872656, Sample = 71503)]
    public object SolvePartTwo(string input)
    {
        var lines = InputHelpers.AsLines(input);
        
        List<Race> races = new List<Race>
        {
            // Get digits from each line, convert that to the number
            new(long.Parse(new string(lines[0].ToCharArray().Where(char.IsDigit).ToArray())),
                long.Parse(new string(lines[1].ToCharArray().Where(char.IsDigit).ToArray())))
        };

        return CalculateNumberOfWaysToWin(races);
    }

    /*
     * Quadratic equation problem. Vb = boat velocity, t = time. 
     *
     * Vb = Th
     * t_moving = t_race - t_hold
     * distance (d) = Vb * t_moving
     * d = t_hold * (t_race - t_hold)
     *
     * Solve for t_hold (we have d and t_race)
     * d = t.race * t.hold - t.hold^2
     * 0 = - t.hold^2 + t.race * t.hold - d
     *
     * We now have a quadratic equation we can solve to get the roots. All integers between these roots (the problem only considers integers)
     * are t.hold which would make the solution exceed the target distance.
     */
    private long CalculateNumberOfWaysToWin(List<Race> races)
    {
        long waysToWin = 1;
        foreach (Race race in races)
        {
            (double root1, double root2) = SolveQuadratic(-1, race.Time, -race.Distance);
            (double upper, double lower) = (Math.Max(root1, root2), Math.Min(root1, root2));
            long waysToWinForRace = GetNumberOfIntegersBetweenDoubles(upper, lower);
            waysToWin *= waysToWinForRace;
        }

        return waysToWin;
    }

    private long GetNumberOfIntegersBetweenDoubles(double upper, double lower)
    {
        // https://math.stackexchange.com/questions/1867236/number-of-integers-between-two-real-numbers
        double fLower = Math.Floor(lower);
        double fUpper = Math.Floor(upper);

        if (fUpper - fLower == 0)
        {
            return 0;
        }

        if (upper - Math.Truncate(upper) == 0)
        {
            return (long)(fUpper - fLower - 1);
        }

        return (long)(fUpper - fLower);
    }

    private (double root1, double root2) SolveQuadratic(long a, long b, long c)
    {
        return (Solve(false), Solve(true));

        double Solve(bool plus)
        {
            long multi = plus ? 1 : -1;
            return (-b + multi * Math.Sqrt(Math.Pow(b, 2) - 4 * a * c)) / (2 * a);
        }
    }

    IEnumerable<long> GetIntegers(string line)
    {
        return line.Split(' ')
            .Where(e => e != "")
            .Skip(1)
            .Select(long.Parse);
    }
}