namespace Sudoku.Core.Domain;

/// <summary>
/// A mutable 9×9 grid of digits. It stores values only; it does not enforce Sudoku rules
/// (see <see cref="Validation.IGridValidator"/>), so a grid may temporarily contain conflicts.
/// </summary>
public sealed class SudokuGrid : IReadOnlyGrid
{
    public const int Size = 9;
    public const int BoxSize = 3;
    public const int Empty = 0;
    public const int MinDigit = 1;
    public const int MaxDigit = 9;

    private readonly int[,] _cells;

    public SudokuGrid()
        : this(new int[Size, Size])
    {
    }

    private SudokuGrid(int[,] cells)
    {
        _cells = cells;
    }

    public int this[Position position]
    {
        get => _cells[position.Row, position.Column];
        set => _cells[position.Row, position.Column] = IsDigit(value) || value == Empty
            ? value
            : throw new ArgumentOutOfRangeException(nameof(value), value, $"Must be {Empty} (empty) or a digit {MinDigit}-{MaxDigit}.");
    }

    public bool IsFull => Position.All.All(position => !IsEmpty(position));

    public int FilledCount => Position.All.Count(position => !IsEmpty(position));

    public static bool IsDigit(int value) => value is >= MinDigit and <= MaxDigit;

    /// <summary>
    /// Builds a grid from 81 cell characters in row-major order. Digits 1–9 are values;
    /// '0', '.' and '_' are empty cells. Whitespace is ignored, so rows may be laid out on separate lines.
    /// </summary>
    public static SudokuGrid Parse(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        var symbols = text.Where(c => !char.IsWhiteSpace(c)).ToArray();
        if (symbols.Length != Size * Size)
        {
            throw new FormatException($"Expected {Size * Size} cells but found {symbols.Length}.");
        }

        var grid = new SudokuGrid();
        foreach (var position in Position.All)
        {
            grid[position] = ParseCell(symbols[position.Row * Size + position.Column]);
        }

        return grid;
    }

    public bool IsEmpty(Position position) => this[position] == Empty;

    public SudokuGrid Clone() => new((int[,])_cells.Clone());

    private static int ParseCell(char symbol) => symbol switch
    {
        '0' or '.' or '_' => Empty,
        >= '1' and <= '9' => symbol - '0',
        _ => throw new FormatException($"Invalid cell symbol '{symbol}'."),
    };
}
