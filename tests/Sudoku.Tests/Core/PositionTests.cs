using Sudoku.Core.Domain;

namespace Sudoku.Tests.Core;

public class PositionTests
{
    [Theory]
    [InlineData("A1", 0, 0)]
    [InlineData("B3", 1, 2)]
    [InlineData("i9", 8, 8)]
    [InlineData("e5", 4, 4)]
    public void TryParse_ValidReference_ReturnsZeroBasedPosition(string text, int row, int column)
    {
        Assert.True(Position.TryParse(text, out var position));
        Assert.Equal(new Position(row, column), position);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("J1")]
    [InlineData("A0")]
    [InlineData("A10")]
    [InlineData("3B")]
    [InlineData("AA")]
    public void TryParse_InvalidReference_ReturnsFalse(string? text)
    {
        Assert.False(Position.TryParse(text, out _));
    }

    [Fact]
    public void ToString_UsesRowLetterAndOneBasedColumn()
    {
        Assert.Equal("C5", new Position(2, 4).ToString());
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(1, 2, 0)]
    [InlineData(0, 8, 2)]
    [InlineData(4, 4, 4)]
    [InlineData(8, 0, 6)]
    [InlineData(8, 8, 8)]
    public void Box_IsNumberedLeftToRightTopToBottom(int row, int column, int expectedBox)
    {
        Assert.Equal(expectedBox, new Position(row, column).Box);
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(0, 9)]
    public void Constructor_OutOfRange_Throws(int row, int column)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Position(row, column));
    }

    [Fact]
    public void All_ContainsEveryCellOnce()
    {
        Assert.Equal(81, Position.All.Distinct().Count());
    }
}
