using Sudoku.Core.Domain;
using Sudoku.Core.Solving;
using Sudoku.Core.Validation;
using Sudoku.Tests.TestDoubles;

namespace Sudoku.Tests.Core;

public class BacktrackingSolverTests
{
    private readonly BacktrackingSolver _solver = new();

    [Fact]
    public void Solve_SamplePuzzle_ReturnsKnownSolution()
    {
        var solved = _solver.Solve(SudokuGrid.Parse(SamplePuzzle.Givens));

        Assert.NotNull(solved);
        var expected = SudokuGrid.Parse(SamplePuzzle.Solution);
        Assert.All(Position.All, p => Assert.Equal(expected[p], solved[p]));
    }

    [Fact]
    public void Solve_DoesNotModifyInput()
    {
        var input = SudokuGrid.Parse(SamplePuzzle.Givens);

        _solver.Solve(input);

        Assert.Equal(30, input.FilledCount);
    }

    [Fact]
    public void Solve_EmptyGrid_ProducesValidCompleteGrid()
    {
        var solved = new BacktrackingSolver(new Random(42)).Solve(new SudokuGrid());

        Assert.NotNull(solved);
        Assert.True(solved.IsFull);
        Assert.Empty(new GridValidator().Validate(solved));
    }

    [Fact]
    public void Solve_GridWithConflict_ReturnsNull()
    {
        var grid = new SudokuGrid();
        grid[new Position(0, 0)] = 1;
        grid[new Position(0, 1)] = 1;

        Assert.Null(_solver.Solve(grid));
    }

    [Fact]
    public void Solve_UnsolvableGridWithoutDirectConflict_ReturnsNull()
    {
        // A1 cannot hold any digit: 1–8 are in row A and 9 is in column 1.
        var grid = SudokuGrid.Parse("""
            _12345678
            9________
            _________
            _________
            _________
            _________
            _________
            _________
            _________
            """);

        Assert.Null(_solver.Solve(grid));
    }

    [Fact]
    public void CountSolutions_UniquePuzzle_ReturnsOne()
    {
        Assert.Equal(1, _solver.CountSolutions(SudokuGrid.Parse(SamplePuzzle.Givens), limit: 2));
    }

    [Fact]
    public void CountSolutions_StopsAtLimit()
    {
        Assert.Equal(2, _solver.CountSolutions(new SudokuGrid(), limit: 2));
    }

    [Fact]
    public void CountSolutions_InvalidLimit_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _solver.CountSolutions(new SudokuGrid(), limit: 0));
    }
}
