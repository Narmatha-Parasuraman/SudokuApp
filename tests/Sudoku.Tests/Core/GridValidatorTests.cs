using Sudoku.Core.Domain;
using Sudoku.Core.Validation;
using Sudoku.Tests.TestDoubles;

namespace Sudoku.Tests.Core;

public class GridValidatorTests
{
    private readonly GridValidator _validator = new();

    [Fact]
    public void EmptyGrid_HasNoViolations()
    {
        Assert.Empty(_validator.Validate(new SudokuGrid()));
    }

    [Fact]
    public void PuzzleGivens_HaveNoViolations()
    {
        Assert.Empty(_validator.Validate(SudokuGrid.Parse(SamplePuzzle.Givens)));
    }

    [Fact]
    public void SolvedGrid_HasNoViolations()
    {
        Assert.Empty(_validator.Validate(SudokuGrid.Parse(SamplePuzzle.Solution)));
    }

    [Fact]
    public void DuplicateInRow_IsReported()
    {
        var grid = new SudokuGrid();
        grid[new Position(0, 0)] = 3;
        grid[new Position(0, 8)] = 3;

        Assert.Equal([new RuleViolation(UnitType.Row, 0, 3)], _validator.Validate(grid));
    }

    [Fact]
    public void DuplicateInColumn_IsReported()
    {
        var grid = new SudokuGrid();
        grid[new Position(0, 4)] = 5;
        grid[new Position(8, 4)] = 5;

        Assert.Equal([new RuleViolation(UnitType.Column, 4, 5)], _validator.Validate(grid));
    }

    [Fact]
    public void DuplicateInBoxOnly_IsReported()
    {
        var grid = new SudokuGrid();
        grid[new Position(3, 3)] = 8;
        grid[new Position(5, 5)] = 8;

        Assert.Equal([new RuleViolation(UnitType.Box, 4, 8)], _validator.Validate(grid));
    }

    [Fact]
    public void DuplicateSharingRowAndBox_IsReportedForEach_RowsFirst()
    {
        // Problem statement example: A3 = 3 alongside the given A2 = 3.
        var grid = SudokuGrid.Parse(SamplePuzzle.Givens);
        grid[new Position(0, 2)] = 3;

        Assert.Equal(
            [new RuleViolation(UnitType.Row, 0, 3), new RuleViolation(UnitType.Box, 0, 3)],
            _validator.Validate(grid));
    }

    [Fact]
    public void ValidatorDoesNotCompareAgainstTheSolution()
    {
        // A wrong-but-legal entry is not a rule violation.
        var grid = SudokuGrid.Parse(SamplePuzzle.Givens);
        grid[new Position(0, 2)] = 1; // Solution is 4.

        Assert.Empty(_validator.Validate(grid));
    }
}
