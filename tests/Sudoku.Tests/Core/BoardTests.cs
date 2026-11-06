using Sudoku.Core.Domain;
using Sudoku.Tests.TestDoubles;

namespace Sudoku.Tests.Core;

public class BoardTests
{
    private static readonly Position PreFilled = new(0, 0);  // A1 = 5
    private static readonly Position EmptyCell = new(0, 2); // A3

    private readonly Board _board = new(SamplePuzzle.Create());

    [Fact]
    public void Place_InEmptyCell_IsAccepted()
    {
        Assert.Equal(MoveResult.Accepted, _board.Place(EmptyCell, 4));
        Assert.Equal(4, _board.Grid[EmptyCell]);
    }

    [Fact]
    public void Place_ConflictingNumber_IsStillAccepted()
    {
        // Conflicts are reported by "check", not blocked at entry time.
        Assert.Equal(MoveResult.Accepted, _board.Place(EmptyCell, 3));
    }

    [Fact]
    public void Place_OverOwnEntry_ReplacesIt()
    {
        _board.Place(EmptyCell, 1);
        _board.Place(EmptyCell, 4);

        Assert.Equal(4, _board.Grid[EmptyCell]);
    }

    [Fact]
    public void Place_InPreFilledCell_IsRejected()
    {
        Assert.Equal(MoveResult.CellIsPreFilled, _board.Place(PreFilled, 6));
        Assert.Equal(5, _board.Grid[PreFilled]);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(-3)]
    public void Place_NumberOutsideOneToNine_IsRejected(int number)
    {
        Assert.Equal(MoveResult.NumberOutOfRange, _board.Place(EmptyCell, number));
        Assert.True(_board.Grid.IsEmpty(EmptyCell));
    }

    [Fact]
    public void Clear_UserEntry_EmptiesCell()
    {
        _board.Place(EmptyCell, 4);

        Assert.Equal(MoveResult.Accepted, _board.Clear(EmptyCell));
        Assert.True(_board.Grid.IsEmpty(EmptyCell));
    }

    [Fact]
    public void Clear_PreFilledCell_IsRejected()
    {
        Assert.Equal(MoveResult.CellIsPreFilled, _board.Clear(PreFilled));
        Assert.Equal(5, _board.Grid[PreFilled]);
    }
}
