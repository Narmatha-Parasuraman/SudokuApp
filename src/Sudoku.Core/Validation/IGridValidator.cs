using Sudoku.Core.Domain;

namespace Sudoku.Core.Validation;

public interface IGridValidator
{
    /// <summary>
    /// Finds every duplicated digit, ordered rows first, then columns, then boxes.
    /// Empty cells are ignored, so a partially filled grid can be valid.
    /// </summary>
    IReadOnlyList<RuleViolation> Validate(IReadOnlyGrid grid);
}
