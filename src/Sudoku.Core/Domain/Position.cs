namespace Sudoku.Core.Domain;

/// <summary>
/// A cell coordinate on the 9×9 board. Rows are labelled A–I and columns 1–9 for display,
/// but are stored as zero-based indexes.
/// </summary>
public readonly record struct Position
{
    private const char FirstRowLabel = 'A';

    public Position(int row, int column)
    {
        Row = EnsureInRange(row, nameof(row));
        Column = EnsureInRange(column, nameof(column));
    }

    /// <summary>Zero-based row index (0 = A).</summary>
    public int Row { get; }

    /// <summary>Zero-based column index (0 = column 1).</summary>
    public int Column { get; }

    /// <summary>Zero-based index of the 3×3 box, numbered left-to-right, top-to-bottom.</summary>
    public int Box => Row / SudokuGrid.BoxSize * SudokuGrid.BoxSize + Column / SudokuGrid.BoxSize;

    public char RowLabel => ToRowLabel(Row);

    public int ColumnNumber => Column + 1;

    /// <summary>Every position on the board in row-major order.</summary>
    public static IEnumerable<Position> All =>
        from row in Enumerable.Range(0, SudokuGrid.Size)
        from column in Enumerable.Range(0, SudokuGrid.Size)
        select new Position(row, column);

    public static char ToRowLabel(int row) => (char)(FirstRowLabel + row);

    /// <summary>Parses a cell reference such as "B3" (case-insensitive).</summary>
    public static bool TryParse(string? text, out Position position)
    {
        position = default;
        if (string.IsNullOrWhiteSpace(text) || text.Length != 2)
        {
            return false;
        }

        var row = char.ToUpperInvariant(text[0]) - FirstRowLabel;
        var column = text[1] - '1';
        if (!IsInRange(row) || !IsInRange(column))
        {
            return false;
        }

        position = new Position(row, column);
        return true;
    }

    public override string ToString() => $"{RowLabel}{ColumnNumber}";

    private static bool IsInRange(int index) => index is >= 0 and < SudokuGrid.Size;

    private static int EnsureInRange(int index, string paramName) =>
        IsInRange(index)
            ? index
            : throw new ArgumentOutOfRangeException(paramName, index, $"Must be between 0 and {SudokuGrid.Size - 1}.");
}
