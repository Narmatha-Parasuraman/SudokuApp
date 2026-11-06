using Sudoku.Core.Domain;

namespace Sudoku.Core.Hints;

/// <summary>The correct value for one cell.</summary>
public sealed record Hint(Position Position, int Value);

public interface IHintProvider
{
    /// <summary>Returns a hint, or <c>null</c> if every cell already holds its correct value.</summary>
    Hint? GetHint(Board board);
}
