namespace AdventOfCode.Lib.Helpers;

public record Cell(char Value, int Row, int Column)
{
    public bool Neighbours8Direction(Cell other) => (other.Row >= Row - 1 && other.Row <= Row + 1) && (other.Column >= Column - 1 && other.Column <= Column + 1); 
    public bool Neighbours4Direction(Cell other) => NeighboursX(other) && NeighboursY(other);
    public bool NeighboursX(Cell other) => other.Row == Row && Math.Abs(other.Column - Column) == 1;
    public bool NeighboursY(Cell other) => other.Column == Column && Math.Abs(other.Row - Row) == 1;
}

public class Grid
{
    private readonly Cell[][] _cells;

    public Grid(char[][] elements)
    {
        _cells = elements
            .Select((row, rowIndex) => row
                .Select((column, columnIndex) => new Cell(column, rowIndex, columnIndex))
                .ToArray())
            .ToArray();
    }

    public IEnumerable<Cell> Cells()
    {
        return _cells.SelectMany(row => row);
    }

    public char Get(int column, int row)
    {
        return _cells[row][column].Value;
    }

    public IEnumerable<Cell> GetRow(int row)
    {
        return _cells[row];
    }

    public IEnumerable<Cell> GetColumn(int column)
    {
        return _cells.Select(row => row[column]);
    }
}