using AdventOfCode.Lib;
using AdventOfCode.Lib.Attributes;
using AdventOfCode.Lib.Helpers;

namespace AdventOfCode.Y2023.D03;

public class Y2023D03 : ISolution
{
    [ExpectedResult(551094, Sample = 4361)]
    public object SolvePartOne(string input)
    {
        char[] symbols = input
            .Where(c => !char.IsDigit(c) && c != '.')
            .Distinct()
            .ToArray();

        Grid grid = InputHelpers.AsGrid(input);
        var symbolCells = grid.Cells().Where(cell => symbols.Contains(cell.Value));

        List<List<Cell>> numberCellGroups = GetNumberCellGroups(grid);

        List<List<Cell>> numberCellGroupsWhichNeighbourSymbols = numberCellGroups
            .Where(cellGroup => symbolCells.Any(symbolCell => cellGroup.Any(cell => cell.IsNeighbour8Direction(symbolCell))))
            .ToList();

        List<int> numbersNeighboringSymbols = numberCellGroupsWhichNeighbourSymbols
            .Select(cg => new string(cg.Select(c => c.Value).ToArray()))
            .Select(int.Parse)
            .ToList();

        return numbersNeighboringSymbols.Sum();
    }

    [ExpectedResult(80179647, Sample = 467835)]
    public object SolvePartTwo(string input)
    {
        Grid grid = InputHelpers.AsGrid(input);

        List<List<Cell>> numberCellGroups = GetNumberCellGroups(grid);

        IEnumerable<Cell> gearCells = grid.Cells().Where(c => c.Value == '*');

        int ratioSum = 0;
        foreach (Cell gearCell in gearCells)
        {
            var adjacentNumberGroups = numberCellGroups
                .Where(group => group.Any(groupCell => groupCell.IsNeighbour8Direction(gearCell)))
                .ToList();

            if (adjacentNumberGroups.Count < 2)
            {
                continue;
            }

            ratioSum += adjacentNumberGroups
                .Select(cg => new string(cg.Select(c => c.Value).ToArray()))
                .Select(int.Parse)
                .Aggregate(1, (accum, element) => accum * element);
        }

        return ratioSum;
    }

    private List<List<Cell>> GetNumberCellGroups(Grid grid)
    {
        var digitCells = grid.Cells().Where(cell => char.IsDigit(cell.Value)).ToList();

        List<List<Cell>> numberCellGroups = new();
        List<Cell> currentGroup = new();
        foreach (var currentCell in digitCells)
        {
            if (currentGroup.Count == 0)
            {
                currentGroup.Add(currentCell);
                continue;
            }

            if (currentCell.IsNeighbourX(currentGroup.Last()))
            {
                currentGroup.Add(currentCell);
                continue;
            }

            numberCellGroups.Add(currentGroup);
            currentGroup = new List<Cell>()
            {
                currentCell
            };
        }

        // Add the last group
        if (currentGroup.Any())
        {
            numberCellGroups.Add(currentGroup);
        }

        return numberCellGroups;
    }
}