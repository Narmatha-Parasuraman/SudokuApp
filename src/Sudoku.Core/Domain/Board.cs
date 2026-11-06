namespace Sudoku.Core.Domain;

/// <summary>The outcome of trying to change a cell on the <see cref="Board"/>.</summary>
public enum MoveResult
{
    Accepted,
    CellIsPreFilled,
    NumberOutOfRange,
}

/// <summary>
/// The player's working copy of a puzzle. Enforces the move rules (pre-filled cells are locked and
/// values must be 1–9) but deliberately allows conflicting entries, which the player can find with a check.
/// </summary>
public sealed class Board
{
    private readonly Puzzle _puzzle;
    private readonly SudokuGrid _grid;

    public Board(Puzzle puzzle)
    {
        _puzzle = puzzle ?? throw new ArgumentNullException(nameof(puzzle));
        _grid = puzzle.CreateStartingGrid();
    }

    public IReadOnlyGrid Grid => _grid;

    public Puzzle Puzzle => _puzzle;

    public bool IsGiven(Position position) => _puzzle.IsGiven(position);

    public MoveResult Place(Position position, int number)
    {
        if (IsGiven(position))
        {
            return MoveResult.CellIsPreFilled;
        }

        if (!SudokuGrid.IsDigit(number))
        {
            return MoveResult.NumberOutOfRange;
        }

        _grid[position] = number;
        return MoveResult.Accepted;
    }

    public MoveResult Clear(Position position)
    {
        if (IsGiven(position))
        {
            return MoveResult.CellIsPreFilled;
        }

        _grid[position] = SudokuGrid.Empty;
        return MoveResult.Accepted;
    }
}
