using Sudoku.Core.Domain;
using Sudoku.Core.Hints;
using Sudoku.Tests.TestDoubles;

namespace Sudoku.Tests.Core;

public class RandomHintProviderTests
{
    private readonly RandomHintProvider _provider = new(new Random(1));
    private readonly Board _board = new(SamplePuzzle.Create());

    [Fact]
    public void GetHint_RevealsCorrectValueForAnEmptyCell()
    {
        var hint = _provider.GetHint(_board);

        Assert.NotNull(hint);
        Assert.True(_board.Grid.IsEmpty(hint.Position));
        Assert.Equal(_board.Puzzle.Solution[hint.Position], hint.Value);
    }

    [Fact]
    public void GetHint_DoesNotChangeTheBoard()
    {
        _provider.GetHint(_board);

        Assert.Equal(30, Position.All.Count(p => !_board.Grid.IsEmpty(p)));
    }

    [Fact]
    public void GetHint_NoEmptyCells_PointsAtAnIncorrectEntry()
    {
        FillWithSolution();
        var wrongCell = new Position(0, 2);
        _board.Place(wrongCell, 1); // Correct value is 4.

        Assert.Equal(new Hint(wrongCell, 4), _provider.GetHint(_board));
    }

    [Fact]
    public void GetHint_SolvedBoard_ReturnsNull()
    {
        FillWithSolution();

        Assert.Null(_provider.GetHint(_board));
    }

    private void FillWithSolution()
    {
        foreach (var (position, value) in SamplePuzzle.SolvingMoves())
        {
            _board.Place(position, value);
        }
    }
}
