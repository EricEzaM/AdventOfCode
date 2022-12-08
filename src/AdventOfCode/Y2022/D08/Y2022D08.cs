using AdventOfCode.Lib;
using AdventOfCode.Lib.Attributes;

namespace AdventOfCode.Y2022.D08;

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

    [ExpectedResult(1870, Sample = 21)]
    public object SolvePartOne(string input)
    {
        int gw = input.IndexOf('\n');
        int[] trees = InputHelpers.AsLines(input)
                                  .SelectMany(l => l.Select(c => int.Parse(c.ToString())))
                                  .ToArray();

        return trees.Select((_, i) => GetTreeStats(trees, i, gw))
             .Count(s => s.visibleFromEdge);
    }

    [ExpectedResult(517440, Sample = 8)]
    public object SolvePartTwo(string input)
    {
        int gw = input.IndexOf('\n');
        int[] trees = InputHelpers.AsLines(input)
                                  .SelectMany(l => l.Select(c => int.Parse(c.ToString())))
                                  .ToArray();

        return trees.Select((_, i) => GetTreeStats(trees, i, gw))
                    .Max(s => s.score);
    }

    private (bool visibleFromEdge, int score) GetTreeStats(int[] trees, int i, int gw)
    {
        bool visible = false;
        Dictionary<Direction, int> currScores = _dirArr.ToDictionary(dir => dir, _ => 0);

        bool done = false;
        int checkRadius = 1;
        Direction directionsBlocked = 0;

        while (!done)
        {
            foreach (var dir in _dirArr.Where(d => (directionsBlocked & d) != d))
            {
                // Return -1 as the height if:
                // * The location to check is out of bounds of the array
                // * Going left/right would put you into a different row of trees
                int height = dir switch
                {
                    Direction.Left => (i - checkRadius) < 0 || i / gw != (i - checkRadius) / gw
                        ? -1
                        : trees[i - checkRadius],
                    Direction.Right => (i + checkRadius) < 0 || i / gw != (i + checkRadius) / gw
                        ? -1
                        : trees[i + checkRadius],
                    Direction.Top => i - checkRadius * gw < 0
                        ? -1
                        : trees[i - checkRadius * gw],
                    Direction.Bottom => (i + checkRadius * gw) > trees.Length - 1 
                        ? -1
                        : trees[i + checkRadius * gw],
                    _ => throw new ArgumentOutOfRangeException()
                };

                if (height == -1)
                {
                    visible = visible || (directionsBlocked & dir) != dir || checkRadius == 1;
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

        return (visible, currScores.Values.Aggregate((tot, v) => tot * v));
    }
}