using Sudoku.Core.Domain;

namespace Sudoku.Core.Validation;

public sealed class GridValidator : IGridValidator
{
    private static readonly UnitType[] UnitOrder = [UnitType.Row, UnitType.Column, UnitType.Box];

    public IReadOnlyList<RuleViolation> Validate(IReadOnlyGrid grid)
    {
        ArgumentNullException.ThrowIfNull(grid);

        return UnitOrder
            .SelectMany(unit => Enumerable.Range(0, SudokuGrid.Size)
                .SelectMany(index => FindDuplicates(grid, unit, index)))
            .ToList();
    }

    private static IEnumerable<RuleViolation> FindDuplicates(IReadOnlyGrid grid, UnitType unit, int index) =>
        GridUnits.CellsIn(unit, index)
            .Where(position => !grid.IsEmpty(position))
            .GroupBy(position => grid[position])
            .Where(group => group.Count() > 1)
            .OrderBy(group => group.Key)
            .Select(group => new RuleViolation(unit, index, group.Key));
}
