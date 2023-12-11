namespace AdventOfCode.Lib.Helpers;

public class Cell : Cell<char>
{
    public Cell()
    {
    }
    
    public Cell(char value, int row, int column) : base(value, row, column)
    {
    }
}

public class Grid : Grid<Cell, char>
{
    public Grid(char[][] elements) : base(elements)
    {
    }
}

public class Cell<T> where T : struct
{
    public Cell()
    {
    }

    public Cell(T value, int row, int column)
    {
        Value = value;
        Row = row;
        Column = column;
    }

    public T Value { get; init; }
    public int Row { get; init; }
    public int Column { get; init; }

    public bool IsNeighbour8Direction(Cell<T> other) => (other.Row >= Row - 1 && other.Row <= Row + 1) && (other.Column >= Column - 1 && other.Column <= Column + 1);
    public bool IsNeighbour4Direction(Cell<T> other) => IsNeighbourX(other) || IsNeighbourY(other);
    public bool IsNeighbourX(Cell<T> other) => other.Row == Row && Math.Abs(other.Column - Column) == 1;
    public bool IsNeighbourY(Cell<T> other) => other.Column == Column && Math.Abs(other.Row - Row) == 1;
}

public class Grid<TCell, TValue>
    where TCell : Cell<TValue>, new()
    where TValue : struct
{
    private readonly TCell[][] _cells;

    public Grid(TValue[][] elements)
    {
        _cells = elements
            .Select((row, rowIndex) => row
                .Select((column, columnIndex) => new TCell()
                {
                    Value = column,
                    Row = rowIndex,
                    Column = columnIndex
                })
                .ToArray())
            .ToArray();
    }

    public IEnumerable<TCell> Cells()
    {
        return _cells.SelectMany(row => row);
    }

    public TValue Get(int column, int row)
    {
        return _cells[row][column].Value;
    }

    public IEnumerable<TCell> GetRow(int row)
    {
        return _cells[row];
    }

    public IEnumerable<TCell> GetColumn(int column)
    {
        return _cells.Select(row => row[column]);
    }

    public IEnumerable<TCell> GetNeighbours(TCell cell, bool x = true, bool y = true, bool diag = false)
    {
        if (x)
        {
            if (cell.Column < _cells[0].Length - 1)
            {
                yield return _cells[cell.Row][cell.Column + 1];
            }

            if (cell.Column > 0)
            {
                yield return _cells[cell.Row][cell.Column - 1];
            }
        }
        if (y)
        {
            if (cell.Row < _cells.Length - 1)
            {
                yield return _cells[cell.Row + 1][cell.Column];
            }

            if (cell.Row > 0)
            {
                yield return _cells[cell.Row - 1][cell.Column];
            }
        }

        if (diag)
        {
            // TODO
        }
    }
}