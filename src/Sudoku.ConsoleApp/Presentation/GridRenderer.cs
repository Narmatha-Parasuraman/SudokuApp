using System.Text;
using Sudoku.Core.Domain;

namespace Sudoku.ConsoleApp.Presentation;

/// <summary>
/// Renders a grid as text with column numbers across the top and row letters down the side:
/// <code>
///     1 2 3 4 5 6 7 8 9
///   A 5 3 _ _ 7 _ _ _ _
/// </code>
/// </summary>
public static class GridRenderer
{
    private const string EmptyCell = "_";
    private const string RowIndent = "  ";

    public static string Render(IReadOnlyGrid grid)
    {
        ArgumentNullException.ThrowIfNull(grid);

        var builder = new StringBuilder();
        builder.Append(RowIndent).Append("  ").AppendJoin(' ', Enumerable.Range(1, SudokuGrid.Size));

        for (var row = 0; row < SudokuGrid.Size; row++)
        {
            var cells = Enumerable.Range(0, SudokuGrid.Size).Select(column => RenderCell(grid, new Position(row, column)));
            builder.AppendLine()
                .Append(RowIndent).Append(Position.ToRowLabel(row)).Append(' ')
                .AppendJoin(' ', cells);
        }

        return builder.ToString();
    }

    private static string RenderCell(IReadOnlyGrid grid, Position position) =>
        grid.IsEmpty(position) ? EmptyCell : grid[position].ToString();
}
