using AdventOfCode.Lib;
using AdventOfCode.Lib.Attributes;

namespace AdventOfCode.Y2022.D08;

/// <summary>
/// Pretty disgusting
/// </summary>
public class Y2022D08 : ISolution
{
    [Flags]
    enum Direction
    {
        Left = 1 << 0,
        Right = 1 << 1,
        Top = 1 << 2,
        Bottom = 1 << 3,
        All = Left | Right | Top | Bottom
    }

    private readonly Direction[] _dirArr = {Direction.Left, Direction.Right, Direction.Top, Direction.Bottom};

    [ExpectedResult("", Sample = 21)]
    public object SolvePartOne(string input)
    {
        string[] lines = InputHelpers.AsLines(input)
                                     .ToArray();

        int gw = lines[0].Length;

        var trees = lines.SelectMany(l => l.Select(c => int.Parse(c.ToString())))
                         .ToArray();

        int visible = 0;
        Direction directionsBlocked = 0;

        for (int i = 0; i < trees.Length; i++)
        {
            if (i % gw == 0 || i < gw || i % gw == gw - 1 || i > (gw - 1) * gw)
            {
                // Borders/perimeter
                visible++;
                continue;
            }

            int checkRadius = 1;
            bool done = false;
            while (!done)
            {
                foreach (var dir in _dirArr.Where(d => (directionsBlocked & d) != d))
                {
                    var height = dir switch
                    {
                        Direction.Left => i / gw != (i - checkRadius) / gw ? -1 : trees[i - checkRadius],
                        Direction.Right => i / gw != (i + checkRadius) / gw ? -1 : trees[i + checkRadius],
                        Direction.Top => i - checkRadius * gw < 0 ? -1 : trees[i - checkRadius * gw],
                        Direction.Bottom => i + checkRadius * gw > trees.Length ? -1 : trees[i + checkRadius * gw],
                        _ => throw new ArgumentOutOfRangeException()
                    };

                    if (height == -1 && (directionsBlocked &= dir) != dir)
                    {
                        visible++;
                        done = true;
                        break;
                    }

                    if (height >= trees[i])
                    {
                        directionsBlocked |= dir;
                    }

                    if (directionsBlocked == Direction.All)
                    {
                        done = true;
                        break;
                    }
                }

                checkRadius++;
            }

            directionsBlocked = 0;
        }


        return visible;
    }

    [ExpectedResult("", Sample = 8)]
    public object SolvePartTwo(string input)
    {
        // Flatten
        string[] lines = InputHelpers.AsLines(input)
                                     .ToArray();

        int gw = lines[0].Length;

        var trees = lines.SelectMany(l => l.Select(c => int.Parse(c.ToString())))
                         .ToArray();

        int bestScore = 0;
        Dictionary<Direction, int> currScores = _dirArr.ToDictionary(dir => dir, _ => 0);
        Direction directionsBlocked = 0;

        for (int i = 0; i < trees.Length; i++)
        {
            int checkRadius = 1;
            bool done = false;
            while (!done)
            {
                foreach (var dir in _dirArr.Where(d => (directionsBlocked & d) != d))
                {
                    int height = dir switch
                    {
                        Direction.Left => (i - checkRadius) < 0 || i / gw != (i - checkRadius) / gw ? -1 : trees[i - checkRadius],
                        Direction.Right => (i + checkRadius) < 0 || i / gw != (i + checkRadius) / gw ? -1 : trees[i + checkRadius],
                        Direction.Top => (i - checkRadius * gw) > trees.Length - 1 || i - checkRadius * gw < 0 ? -1 : trees[i - checkRadius * gw],
                        Direction.Bottom => (i + checkRadius * gw) > trees.Length - 1 || i + checkRadius * gw > trees.Length ? -1 : trees[i + checkRadius * gw],
                        _ => throw new ArgumentOutOfRangeException()
                    };

                    if (height == -1)
                    {
                        directionsBlocked |= dir;
                    }
                    else if (height >= trees[i])
                    {
                        currScores[dir]++;
                        directionsBlocked |= dir;
                    }
                    else
                    {
                        currScores[dir]++;
                    }

                    if (directionsBlocked == Direction.All)
                    {
                        done = true;
                        break;
                    }
                }

                checkRadius++;
            }

            var score = currScores.Values.Aggregate((tot, v) => tot * v);
            if (score > bestScore)
            {
                Console.WriteLine($"new best tree {i}: {score}");
                bestScore = score;
            }
            
            currScores = _dirArr.ToDictionary(dir => dir, _ => 0);
            directionsBlocked = 0;
        }


        return bestScore;
    }
}