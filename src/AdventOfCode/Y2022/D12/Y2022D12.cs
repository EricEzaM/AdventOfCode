using System.Diagnostics;
using AdventOfCode.Lib;
using AdventOfCode.Lib.Attributes;
using AdventOfCode.Lib.Helpers;
using Spectre.Console;

namespace AdventOfCode.Y2022.D12;

public class Y2022D12 : ISolution
{
    [DebuggerDisplay("{Value}: {X},{Y}")]
    private class Cell
    {
        public char Value { get; }
        public int X { get; }
        public int Y { get; }

        public Cell(char value, int x, int y)
        {
            Value = value;
            X = x;
            Y = y;
        }
    }
    
    private class Graph
    {
        public List<Cell> Cells { get; } = new();

        public Graph(char[][] grid)
        {
            for (int row = 0; row < grid.Length; row++)
            {
                for (int col = 0; col < grid[row].Length; col++)
                {
                    Cells.Add(new Cell(grid[row][col], col, row));
                }
            }
        }

        public void PrintPath(List<Cell> path)
        {
            Dictionary<int, HashSet<int>> cellsOnPath = path.GroupBy(c => c.Y)
                                                            .ToDictionary(
                                                                g => g.Key, g => new HashSet<int>(g.Select(c => c.X)));

            foreach (var cell in Cells)
            {
                if (cell.X == 0)
                {
                    AnsiConsole.WriteLine();
                }

                if (cellsOnPath.ContainsKey(cell.Y) && cellsOnPath[cell.Y].Contains(cell.X))
                {
                    AnsiConsole.Markup("[green]{0}[/]", cell.Value);
                }
                else
                {
                    AnsiConsole.Markup("[red]{0}[/]", cell.Value);
                }
            }

            AnsiConsole.WriteLine();
        }

        public IEnumerable<Cell> GetNeighbors(Cell cell)
        {
            return Cells.Where(c => c.Y == cell.Y && Math.Abs(cell.X - c.X) == 1 ||
                                    c.X == cell.X && Math.Abs(cell.Y - c.Y) == 1);
        }

        public List<Cell> AStar(Cell start, Cell end, Func<Cell, Cell, int> heuristic,
                                Func<Cell, Cell, int?> edgeWeight)
        {
            var openSet = new PriorityQueue<Cell, int>();
            openSet.Enqueue(start, 0);

            Dictionary<Cell, Cell> cameFrom = new();

            Dictionary<Cell, int> gScore = new()
            {
                {start, 0}
            };

            Dictionary<Cell, int> fScore = new()
            {
                {start, heuristic(start, end)}
            };

            while (openSet.Count > 0)
            {
                var current = openSet.Dequeue();
                if (current == end)
                {
                    return ReconstructPath(cameFrom, current);
                }

                foreach (Cell neighbor in GetNeighbors(current))
                {
                    var weight = edgeWeight(current, neighbor);
                    if (weight is null)
                    {
                        continue;
                    }

                    var tentativeGScore = gScore[current] + weight.Value;
                    if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeGScore;
                        fScore[neighbor] = tentativeGScore + heuristic(neighbor, end);
                        openSet.Enqueue(neighbor, fScore[neighbor]);
                    }
                }
            }

            return new List<Cell>();
        }

        private List<Cell> ReconstructPath(Dictionary<Cell, Cell> cameFrom, Cell current)
        {
            List<Cell> path = new() {current};
            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                path.Add(current);
            }

            path.Reverse();
            return path;
        }
    }

    private int EffectiveValue(char c) => c switch
    {
        'S' => 'a',
        'E' => 'z',
        _ => c
    };

    [ExpectedResult(425, Sample = 31)]
    public object SolvePartOne(string input)
    {
        var grid = InputHelpers.AsCharGrid(input);
        var graph = new Graph(grid);
        var path = graph.AStar(graph.Cells.Single(c => c.Value == 'S'),
                               graph.Cells.Single(c => c.Value == 'E'),
                               (curr, end) => Math.Abs(curr.X - end.X) + Math.Abs(curr.Y - end.Y),
                               (curr, neighbor) =>
                               {
                                   var nEff = EffectiveValue(neighbor.Value);
                                   var cEff = EffectiveValue(curr.Value);

                                   var score = nEff > cEff + 1
                                       ? null
                                       : (int?) cEff - nEff + 1;

                                   return score;
                               });

        graph.PrintPath(path);
        return path.Count - 1;
    }

    [ExpectedResult("", Sample = 29)]
    public object SolvePartTwo(string input)
    {
        var grid = InputHelpers.AsCharGrid(input);
        var graph = new Graph(grid);

        var hikeEnd = graph.Cells.Single(c => c.Value == 'E');
        var possibleStarts = graph.Cells
                                  .Where(c => c.Value == 'a')
                                  .Where(c => graph.GetNeighbors(c).Any(n => n.Value == 'b'))
                                  .ToList();

        Console.WriteLine($"{possibleStarts.Count} possible starts");
        
        var paths = new List<List<Cell>>();
        foreach (var start in possibleStarts)
        {
            var path = graph.AStar(start,
                                   hikeEnd,
                                   (curr, end) => Math.Abs(curr.X - end.X) + Math.Abs(curr.Y - end.Y),
                                   (curr, neighbor) =>
                                   {
                                       var nEff = EffectiveValue(neighbor.Value);
                                       var cEff = EffectiveValue(curr.Value);

                                       var score = nEff > cEff + 1
                                           ? null
                                           : (int?) cEff - nEff + 1;

                                       return score;
                                   });

            Console.WriteLine($"Start at {start.X},{start.Y}, path = {path.Count}");

            if (path.Count > 0)
            {
                paths.Add(path);
            }
        }

        var shortest = paths.MinBy(p => p.Count);
        graph.PrintPath(shortest);
        
        return shortest.Count - 1;
    }
}