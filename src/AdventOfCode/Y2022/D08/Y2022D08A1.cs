using AdventOfCode.Lib;
using AdventOfCode.Lib.Attributes;
using AdventOfCode.Lib.Helpers;

namespace AdventOfCode.Y2022.D08;

/// <summary>
/// Alternative solution #1
/// Same as #1 but keeps the calculations for part 2.
/// Obviously shaves a lot of time off - but this is a 'fair' comparison since one method is doing the work for
/// both puzzles.
/// ┌───────┬─────┬─────┬──────┬────────────┬──────────────┬────────┬────────────┬──────────────┐
/// │ Year  │ Day │ Alt │ P1   │ P1 time    │ P1 mem delta │ P2     │ P2 time    │ P2 mem delta │
/// ├───────┼─────┼─────┼──────┼────────────┼──────────────┼────────┼────────────┼──────────────┤
/// │ Y2022 │ D08 │ 0   │ 1870 │ 23.9481 ms │ 7035.8 kB    │ 517440 │ 16.6865 ms │ 7035.82 kB   │
/// │ Y2022 │ D08 │ 1   │ 1870 │ 20.9499 ms │ 7291.86 kB   │ 517440 │ 1.3805 ms  │ 16 kB        │
/// └───────┴─────┴─────┴──────┴────────────┴──────────────┴────────┴────────────┴──────────────┘
/// </summary>
public class Y2022D08A1 : ISolution
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
    private List<(bool visibleFromEdge, int score)> _stats = new();

    [ExpectedResult(1870, Sample = 21)]
    public object SolvePartOne(string input)
    {
        int gw = input.IndexOf('\n');
        int[] trees = InputHelpers.AsLines(input)
                                  .SelectMany(l => l.Select(c => int.Parse(c.ToString())))
                                  .ToArray();

        _stats = trees.Select((_, i) => GetTreeStats(trees, i, gw))
                      .ToList();

        return _stats.Count(s => s.visibleFromEdge);
    }

    [ExpectedResult(517440, Sample = 8)]
    public object SolvePartTwo(string input)
    {
        return _stats.Max(s => s.score);
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
                    Direction.Left => i - checkRadius < 0 || i / gw != (i - checkRadius) / gw
                        ? -1
                        : trees[i - checkRadius],
                    Direction.Right => i + checkRadius < 0 || i / gw != (i + checkRadius) / gw
                        ? -1
                        : trees[i + checkRadius],
                    Direction.Top => i - checkRadius * gw < 0
                        ? -1
                        : trees[i - checkRadius * gw],
                    Direction.Bottom => i + checkRadius * gw > trees.Length - 1
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