using AdventOfCode.Lib;
using AdventOfCode.Lib.Attributes;
using AdventOfCode.Lib.Helpers;

namespace AdventOfCode.Y2022.D14;

public class Y2022D14 : ISolution
{
    private record Point(int X, int Y)
    {
        public static Point operator +(Point a, Point b) => new(a.X + b.X, a.Y + b.Y);
        public static Point operator -(Point a, Point b) => new(a.X - b.X, a.Y - b.Y);

        public static readonly Point Down = new(0, 1);
        public static readonly Point DownLeft = new(-1, 1);
        public static readonly Point DownRight = new(1, 1);
    };

    private readonly Point[] _possibleMoves = {Point.Down, Point.DownLeft, Point.DownRight};

    [ExpectedResult(1330, Sample = 24)]
    public object SolvePartOne(string input)
    {
        var walls = InputHelpers.AsLines(input)
                                .Select(l => l.Split(" -> "))
                                .Select(point => point.Select(xy =>
                                                      {
                                                          var split = xy.Split(',')
                                                                        .Select(int.Parse)
                                                                        .ToArray();
                                                          return new Point(split[0], split[1]);
                                                      })
                                                      .ToArray())
                                .ToArray();


        return GetSettledCount(GetWallPoints(walls), false);
    }

    [ExpectedResult(26139, Sample = 93)]
    public object SolvePartTwo(string input)
    {
        var walls = InputHelpers.AsLines(input)
                                .Select(l => l.Split(" -> "))
                                .Select(point => point.Select(xy =>
                                                      {
                                                          var split = xy.Split(',')
                                                                        .Select(int.Parse)
                                                                        .ToArray();
                                                          return new Point(split[0], split[1]);
                                                      })
                                                      .ToArray())
                                .ToArray();


        return GetSettledCount(GetWallPoints(walls), true);
    }

    private int GetSettledCount(HashSet<Point> walls, bool hasFloor)
    {
        int maxY = walls.Max(p => p.Y);
        int floor = maxY + 2;
        HashSet<Point> sands = new HashSet<Point>();

        bool complete = false;

        Stack<Point> path = new();
        Point current = new Point(500, 0);
        path.Push(current);
        
        while (!complete)
        {
            while (true)
            {
                var nextPoint = _possibleMoves.Select(delta => current + delta)
                                              .FirstOrDefault(tryPoint => !sands.Contains(tryPoint) &&
                                                                          !walls.Contains(tryPoint) &&
                                                                          !(hasFloor && tryPoint.Y >= floor));

                if (nextPoint is null)
                {
                    sands.Add(current);
                    if (path.Count == 0)
                    {
                        complete = true;
                        break;
                    }
                    current = path.Pop();
                    break;
                }

                if (!hasFloor && nextPoint.Y >= maxY)
                {
                    complete = true;
                    break;
                }
                
                path.Push(current);
                current = nextPoint;
            }
        }

        return sands.Count;
    }

    /// <summary>
    /// Gets a list of points that make up all walls, based on an array of 'walls'
    /// made up from an array of points that contain the corner points of the wall.
    /// </summary>
    private HashSet<Point> GetWallPoints(Point[][] walls)
    {
        HashSet<Point> wallPoints = new();
        foreach (var wall in walls)
        {
            for (int i = 0; i < wall.Length - 1; i++)
            {
                Point a = wall[i];
                Point b = wall[i + 1];
                var dx = b.X - a.X;
                var dy = b.Y - a.Y;

                wallPoints.Add(a);
                // Go from a to b 1 point at a time until b is reached.
                while (a != b)
                {
                    a = dx == 0 ? a with {Y = a.Y + Math.Sign(dy)} : a with {X = a.X + Math.Sign(dx)};
                    wallPoints.Add(a);
                }
            }
        }

        return wallPoints;
    }
}