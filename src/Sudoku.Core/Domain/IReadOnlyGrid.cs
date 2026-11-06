namespace Sudoku.Core.Domain;

/// <summary>Read-only view of a Sudoku grid. Empty cells hold <see cref="SudokuGrid.Empty"/>.</summary>
public interface IReadOnlyGrid
{
    int this[Position position] { get; }

    bool IsEmpty(Position position);

    bool IsFull { get; }
}
