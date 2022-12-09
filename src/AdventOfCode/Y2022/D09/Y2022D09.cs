using AdventOfCode.Lib;
using AdventOfCode.Lib.Attributes;

namespace AdventOfCode.Y2022.D09;

public class Y2022D09 : ISolution
{
    private record struct Pos(int X, int Y)
    {
        public static Pos operator +(Pos a, Pos b) => new Pos(a.X + b.X, a.Y + b.Y);
        public static Pos operator -(Pos a, Pos b) => new Pos(a.X - b.X, a.Y - b.Y);
    };


    [ExpectedResult(6498, Sample = 13)]
    public object SolvePartOne(string input)
    {
        var movements = InputHelpers.AsLines(input)
                                    .Select(l => (l[..1], int.Parse(l[2..])))
                                    .ToArray();

        return GetUniquePositions(movements, 2, 1);
    }

    /// <summary>
    /// Note for Part 2 there is a separate more complex sample in addition to the first (smaller) sample.
    /// </summary>
    [ExpectedResult(2531, Sample = "")]
    public object SolvePartTwo(string input)
    {
        var movements = InputHelpers.AsLines(input)
                                    .Select(l => (l[..1], int.Parse(l[2..])))
                                    .ToArray();

        return GetUniquePositions(movements, 10, 9);
    }

    private int GetUniquePositions((string, int)[] headMovements, int numPoints, int trackPointIdx)
    {
        HashSet<Pos> uniquePos = new();

        var points = new Pos[numPoints];

        foreach ((string dir, int dist) in headMovements)
        {
            for (int step = 0; step < dist; step++)
            {
                points[0] += dir switch
                {
                    "L" => new Pos(-1, 0),
                    "R" => new Pos(1, 0),
                    "D" => new Pos(0, -1),
                    "U" => new Pos(0, 1),
                };

                for (int pi = 1; pi < points.Length; pi++)
                {
                    var h = points[pi - 1];
                    var t = points[pi];
                    var delta = h - t;
                    points[pi] += delta switch
                    {
                        {X: > 1, Y: > 1} => delta + new Pos(-1, -1),
                        {X: < -1, Y: < -1} => delta + new Pos(1, 1),
                        {X: < -1, Y: > 1} => delta + new Pos(1, -1),
                        {X: > 1, Y: < -1} => delta + new Pos(-1, 1),
                        {X: > 1} => delta with {X = 1},
                        {X: < -1} => delta with {X = -1},
                        {Y: > 1} => delta with {Y = 1},
                        {Y: < -1} => delta with {Y = -1},
                        _ => new Pos()
                    };

                    if (pi == trackPointIdx)
                    {
                        uniquePos.Add(points[pi]);
                    }
                }
            }
        }

        return uniquePos.Count;
    }
}