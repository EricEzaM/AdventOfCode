using AdventOfCode.Lib;
using AdventOfCode.Lib.Attributes;

namespace AdventOfCode.Y2022.D09;

public class Y2022D09 : ISolution
{
    private record struct Pos(int X, int Y)
    {
        public static Pos operator +(Pos a, Pos b) => new Pos(a.X + b.X, a.Y + b.Y);
    };

    
    [ExpectedResult(6498, Sample = 13)]
    public object SolvePartOne(string input)
    {
        var movements = InputHelpers.AsLines(input)
                                    .Select(l => (l[..1], int.Parse(l[2..])))
                                    .ToArray();

        HashSet<Pos> uniquePos = new();

        var h = new Pos();
        var t = new Pos();
        
        foreach ((string dir, int dist) in movements)
        {
            for (int step = 0; step < dist; step++)
            {
                t += dir switch
                {
                    "L" => h.X < t.X ? new Pos(-1, h.Y - t.Y) : new Pos(),
                    "R" => h.X > t.X ? new Pos(1, h.Y - t.Y) : new Pos(),
                    "D" => h.Y < t.Y ? new Pos(h.X - t.X, -1) : new Pos(),
                    "U" => h.Y > t.Y ? new Pos(h.X - t.X, 1) : new Pos(),
                };
                
                h += dir switch
                {
                    "L" => new Pos(-1, 0),
                    "R" => new Pos(1, 0),
                    "D" => new Pos(0, -1),
                    "U" => new Pos(0, 1),
                };
                
                uniquePos.Add(t);
            }
        }

        return uniquePos.Count;
    }

    [ExpectedResult("", Sample = "")]
    public object SolvePartTwo(string input)
    {
        return string.Empty;
    }
}