using Sudoku.Core.Domain;

namespace Sudoku.Core.Validation;

/// <summary>A digit that appears more than once in the same row, column or box.</summary>
/// <param name="Unit">The kind of group containing the duplicate.</param>
/// <param name="UnitIndex">Zero-based index of the row, column or box.</param>
/// <param name="Number">The duplicated digit.</param>
public sealed record RuleViolation(UnitType Unit, int UnitIndex, int Number);
