namespace Sudoku.Core.Domain;

/// <summary>A puzzle: the pre-filled cells ("givens") together with its full solution.</summary>
public sealed class Puzzle
{
    private readonly SudokuGrid _givens;
    private readonly SudokuGrid _solution;

    public Puzzle(SudokuGrid givens, SudokuGrid solution)
    {
        ArgumentNullException.ThrowIfNull(givens);
        ArgumentNullException.ThrowIfNull(solution);

        if (!solution.IsFull)
        {
            throw new ArgumentException("The solution must be completely filled.", nameof(solution));
        }

        if (Position.All.Any(p => !givens.IsEmpty(p) && givens[p] != solution[p]))
        {
            throw new ArgumentException("Every given must match the solution.", nameof(givens));
        }

        // Defensive copies keep the puzzle immutable from the outside.
        _givens = givens.Clone();
        _solution = solution.Clone();
    }

    public IReadOnlyGrid Givens => _givens;

    public IReadOnlyGrid Solution => _solution;

    public bool IsGiven(Position position) => !_givens.IsEmpty(position);

    /// <summary>Creates the grid a player starts from: a copy of the givens.</summary>
    public SudokuGrid CreateStartingGrid() => _givens.Clone();
}
