using Sudoku.Core.Domain;
using Sudoku.Core.Solving;

namespace Sudoku.Core.Generation;

/// <summary>
/// Generates puzzles with exactly <see cref="ClueCount"/> pre-filled cells and a unique solution.
/// </summary>
/// <remarks>
/// Algorithm: build a random complete grid, then visit the cells in random order and empty each one
/// unless doing so would allow a second solution. Stop once the target clue count is reached.
/// If a particular grid cannot be reduced that far while staying unique, start again with a new grid.
/// </remarks>
public sealed class RandomPuzzleGenerator : IPuzzleGenerator
{
    public const int DefaultClueCount = 30;

    /// <summary>No 9×9 Sudoku with fewer than 17 clues has a unique solution.</summary>
    public const int MinimumClueCount = 17;

    private const int MaxAttempts = 100;

    private readonly ISudokuSolver _solver;
    private readonly Random _random;

    /// <param name="solver">Used both to create the complete grid and to check uniqueness.
    /// Pass a solver that randomises its candidate order to get varied puzzles.</param>
    public RandomPuzzleGenerator(ISudokuSolver solver, Random random, int clueCount = DefaultClueCount)
    {
        _solver = solver ?? throw new ArgumentNullException(nameof(solver));
        _random = random ?? throw new ArgumentNullException(nameof(random));
        ArgumentOutOfRangeException.ThrowIfLessThan(clueCount, MinimumClueCount);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(clueCount, SudokuGrid.Size * SudokuGrid.Size);
        ClueCount = clueCount;
    }

    public int ClueCount { get; }

    public Puzzle Generate()
    {
        for (var attempt = 0; attempt < MaxAttempts; attempt++)
        {
            var solution = _solver.Solve(new SudokuGrid())
                ?? throw new InvalidOperationException("The solver could not fill an empty grid.");

            var givens = RemoveCluesKeepingUniqueSolution(solution);
            if (givens.FilledCount == ClueCount)
            {
                return new Puzzle(givens, solution);
            }
        }

        throw new InvalidOperationException(
            $"Could not generate a unique puzzle with {ClueCount} clues after {MaxAttempts} attempts.");
    }

    private SudokuGrid RemoveCluesKeepingUniqueSolution(SudokuGrid solution)
    {
        var givens = solution.Clone();
        var filled = givens.FilledCount;
        var positions = Position.All.ToArray();
        _random.Shuffle(positions);

        foreach (var position in positions)
        {
            if (filled == ClueCount)
            {
                break;
            }

            var value = givens[position];
            givens[position] = SudokuGrid.Empty;

            if (HasUniqueSolution(givens))
            {
                filled--;
            }
            else
            {
                givens[position] = value;
            }
        }

        return givens;
    }

    private bool HasUniqueSolution(IReadOnlyGrid grid) => _solver.CountSolutions(grid, limit: 2) == 1;
}
