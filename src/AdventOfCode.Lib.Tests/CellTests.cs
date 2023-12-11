using AdventOfCode.Lib.Helpers;

namespace AdventOfCode.Lib.Tests;

public class CellTests
{
    [TestCase(1, 1, true)]
    [TestCase(1, 3, true)]
    [TestCase(3, 3, true)]
    [TestCase(0, 0, false)]
    [TestCase(4, 4, false)]
    public void Neighbour8Direction(int row, int col, bool isNeighbor)
    {
        var cellCenter = new Cell(' ', 2, 2);
        var testCell = new Cell(' ', row, col);
        
        Assert.That(cellCenter.IsNeighbour8Direction(testCell), Is.EqualTo(isNeighbor));
    }
}