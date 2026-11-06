using Sudoku.Core.Domain;
using Sudoku.Tests.TestDoubles;

namespace Sudoku.Tests.Core;

public class SudokuGridTests
{
    [Fact]
    public void NewGrid_IsEmpty()
    {
        var grid = new SudokuGrid();

        Assert.All(Position.All, p => Assert.True(grid.IsEmpty(p)));
        Assert.False(grid.IsFull);
        Assert.Equal(0, grid.FilledCount);
    }

    [Fact]
    public void Parse_ReadsDigitsAndEmptySymbols()
    {
        var grid = SudokuGrid.Parse(SamplePuzzle.Givens);

        Assert.Equal(5, grid[new Position(0, 0)]);
        Assert.True(grid.IsEmpty(new Position(0, 2)));
        Assert.Equal(30, grid.FilledCount);
    }

    [Theory]
    [InlineData("123")]
    [InlineData("x00000000000000000000000000000000000000000000000000000000000000000000000000000000")]
    public void Parse_MalformedText_Throws(string text)
    {
        Assert.Throws<FormatException>(() => SudokuGrid.Parse(text));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(10)]
    public void Indexer_ValueOutOfRange_Throws(int value)
    {
        var grid = new SudokuGrid();

        Assert.Throws<ArgumentOutOfRangeException>(() => grid[new Position(0, 0)] = value);
    }

    [Fact]
    public void Clone_IsIndependentCopy()
    {
        var original = new SudokuGrid();
        var copy = original.Clone();

        copy[new Position(0, 0)] = 7;

        Assert.True(original.IsEmpty(new Position(0, 0)));
    }

    [Fact]
    public void IsFull_TrueWhenEveryCellHasAValue()
    {
        Assert.True(SudokuGrid.Parse(SamplePuzzle.Solution).IsFull);
    }
}
