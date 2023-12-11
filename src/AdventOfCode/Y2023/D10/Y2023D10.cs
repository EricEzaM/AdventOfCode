using AdventOfCode.Lib;
using AdventOfCode.Lib.Attributes;
using AdventOfCode.Lib.Helpers;

namespace AdventOfCode.Y2023.D10;

public class Y2023D10 : ISolution
{
    private struct Pipe
    {
        [Flags]
        public enum Direction
        {
            None = 1 << 0,
            North = 1 << 1,
            South = 1 << 2,
            East = 1 << 3,
            West = 1 << 4,
            All = North | South | East | West
        }

        public char Symbol { get; }
        public Direction Connection { get; }

        public Pipe(char symbol)
        {
            Symbol = symbol;
            Connection = symbol switch
            {
                '|' => Direction.North | Direction.South,
                '-' => Direction.East | Direction.West,
                'L' => Direction.North | Direction.East,
                'J' => Direction.North | Direction.West,
                '7' => Direction.South | Direction.West,
                'F' => Direction.South | Direction.East,
                '.' => Direction.None,
                'S' => Direction.All,
                _ => throw new ArgumentOutOfRangeException(nameof(symbol), symbol, null)
            };
        }
    }

    [ExpectedResult("", Sample = 8)]
    public object SolvePartOne(string input)
    {
        Grid<Cell<Pipe>, Pipe> grid = InputHelpers.AsGrid(input, c => new Pipe(c));

        var start = grid.Cells().First(c => c.Value.Connection == Pipe.Direction.All);

        List<Cell<Pipe>> pipes = new()
        {
            start
        };

        while (true)
        {
            if (pipes.Count > 2 && pipes.Last().IsNeighbour4Direction(start) && DoPipeCellsConnect(pipes.Last(), start))
            {
                break;
            }

            var conn = grid.GetNeighbours(pipes.Last())
                .First(neighbourOfLastCell =>
                    (pipes.Count <= 1 || neighbourOfLastCell != pipes.SkipLast(1).Last()) && // Don't allow connection to where it just was
                    DoPipeCellsConnect(neighbourOfLastCell, pipes.Last()));

            pipes.Add(conn);
        }

        return pipes.Count / 2;
    }

    [ExpectedResult("", Sample = "")]
    public object SolvePartTwo(string input)
    {
        return string.Empty;
    }

    private bool DoPipeCellsConnect(Cell<Pipe> a, Cell<Pipe> b)
    {
        bool connects = false;
        if (a.Value.Connection.HasFlag(Pipe.Direction.South))
        {
            connects = connects || (b.Row == a.Row + 1 && b.Value.Connection.HasFlag(Pipe.Direction.North));
        }

        if (a.Value.Connection.HasFlag(Pipe.Direction.North))
        {
            connects = connects || (b.Row == a.Row - 1 && b.Value.Connection.HasFlag(Pipe.Direction.South));
        }

        if (a.Value.Connection.HasFlag(Pipe.Direction.East))
        {
            connects = connects || (b.Column == a.Column + 1 && b.Value.Connection.HasFlag(Pipe.Direction.West));
        }

        if (a.Value.Connection.HasFlag(Pipe.Direction.West))
        {
            connects = connects || (b.Column == a.Column - 1 && b.Value.Connection.HasFlag(Pipe.Direction.East));
        }

        return connects;
    }
}