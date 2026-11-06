using Sudoku.Core.Domain;
using Sudoku.Tests.TestDoubles;

namespace Sudoku.Tests.Core;

public class PuzzleTests
{
    [Fact]
    public void Constructor_IncompleteSolution_Throws()
    {
        var givens = SudokuGrid.Parse(SamplePuzzle.Givens);

        Assert.Throws<ArgumentException>(() => new Puzzle(givens, givens));
    }

    [Fact]
    public void Constructor_GivenDisagreesWithSolution_Throws()
    {
        var givens = SudokuGrid.Parse(SamplePuzzle.Givens);
        givens[new Position(0, 0)] = 9; // Solution has 5 here.

        Assert.Throws<ArgumentException>(() => new Puzzle(givens, SudokuGrid.Parse(SamplePuzzle.Solution)));
    }

    [Fact]
    public void Constructor_CopiesGrids_SoLaterChangesDoNotLeakIn()
    {
        var givens = SudokuGrid.Parse(SamplePuzzle.Givens);
        var puzzle = new Puzzle(givens, SudokuGrid.Parse(SamplePuzzle.Solution));

        givens[new Position(0, 2)] = 4;

        Assert.False(puzzle.IsGiven(new Position(0, 2)));
    }

    [Fact]
    public void CreateStartingGrid_ContainsOnlyTheGivens()
    {
        var grid = SamplePuzzle.Create().CreateStartingGrid();

        Assert.Equal(30, grid.FilledCount);
    }
}
