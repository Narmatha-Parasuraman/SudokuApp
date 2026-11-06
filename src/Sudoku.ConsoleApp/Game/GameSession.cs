using Sudoku.Core.Domain;
using Sudoku.Core.Hints;
using Sudoku.Core.Validation;

namespace Sudoku.ConsoleApp.Game;

/// <summary>The state and services for a single round of play. Commands operate on a session.</summary>
public sealed class GameSession
{
    public GameSession(Board board, IGridValidator validator, IHintProvider hintProvider)
    {
        Board = board ?? throw new ArgumentNullException(nameof(board));
        Validator = validator ?? throw new ArgumentNullException(nameof(validator));
        HintProvider = hintProvider ?? throw new ArgumentNullException(nameof(hintProvider));
    }

    public Board Board { get; }

    public IGridValidator Validator { get; }

    public IHintProvider HintProvider { get; }

    /// <summary>The puzzle is won when every cell is filled and no rule is broken.</summary>
    public bool IsSolved => Board.Grid.IsFull && Validator.Validate(Board.Grid).Count == 0;
}
