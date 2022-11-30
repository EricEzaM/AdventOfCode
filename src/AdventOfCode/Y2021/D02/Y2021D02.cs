using AdventOfCode.Lib;

namespace AdventOfCode.Y2021.D02;

public class Y2021D02 : ISolution
{
    enum Direction
    {
        forward,
        up,
        down
    }
    
    class SubCommand
    {
        public SubCommand(Direction direction, int distance)
        {
            Direction = direction;
            Distance = distance;
        }

        public Direction Direction { get; }
        public int Distance { get; }
    }
    
    public object SolvePartOne(string input)
    {
        var commands = ParseInput(input);
        var h = CalculateHorizontalOffset(commands);
        var v = CalculateVerticalOffset(commands);

        return h * v;
    }

    public object SolvePartTwo(string input)
    {
        var commands = ParseInput(input);

        int h = 0;
        int v = 0;
        foreach ((int forward, int aim) in GetNextForward(commands))
        {
            h += forward;
            v += forward * aim;
        }

        return h * v;
    }

    private IEnumerable<(int forward, int aim)> GetNextForward(List<SubCommand> commands)
    {
        int aim = 0;
        foreach (var c in commands)
        {
            if (c.Direction is Direction.forward)
            {
                yield return (c.Distance, aim);
                continue;
            }

            aim += (c.Direction is Direction.down ? 1 : -1) * c.Distance;
        }
    }
    
    private int CalculateVerticalOffset(List<SubCommand> commands)
    {
        return commands.Where(c => c.Direction is Direction.down or Direction.up)
                       .Aggregate(0, (agg, c) => agg += (c.Direction is Direction.down ? 1 : -1) * c.Distance);
    }
    
    private int CalculateHorizontalOffset(List<SubCommand> commands)
    {
        return commands.Where(c => c.Direction == Direction.forward)
                .Sum(c => c.Distance);
    }

    private List<SubCommand> ParseInput(string input)
    {
        return input.Split('\n')
                    .Select(s =>
                    {
                        string[] split = s.Split(' ');
                        return new SubCommand(Enum.Parse<Direction>(split[0]), int.Parse(split[1]));
                    })
                    .ToList();
    }
}