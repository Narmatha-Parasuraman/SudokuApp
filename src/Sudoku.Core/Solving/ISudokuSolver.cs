using Sudoku.Core.Domain;

namespace Sudoku.Core.Solving;

public interface ISudokuSolver
{
    /// <summary>Returns a solved copy of <paramref name="grid"/>, or <c>null</c> if it has no solution.</summary>
    SudokuGrid? Solve(IReadOnlyGrid grid);

    /// <summary>
    /// Counts the solutions of <paramref name="grid"/>, stopping once <paramref name="limit"/> is reached.
    /// Use a limit of 2 to test whether a puzzle has a unique solution.
    /// </summary>
    int CountSolutions(IReadOnlyGrid grid, int limit);
}
