namespace Sudoku.Core.Domain;

/// <summary>The kinds of group ("unit") in which each digit may appear only once.</summary>
public enum UnitType
{
    Row,
    Column,
    Box,
}

/// <summary>Enumerates the cells that make up each row, column and 3×3 box.</summary>
public static class GridUnits
{
    public static IEnumerable<Position> CellsIn(UnitType unit, int index) => unit switch
    {
        UnitType.Row => Position.All.Where(p => p.Row == index),
        UnitType.Column => Position.All.Where(p => p.Column == index),
        UnitType.Box => Position.All.Where(p => p.Box == index),
        _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, null),
    };

    /// <summary>All cells that share a row, column or box with <paramref name="position"/>, excluding itself.</summary>
    public static IEnumerable<Position> PeersOf(Position position) =>
        Position.All.Where(p => p != position
            && (p.Row == position.Row || p.Column == position.Column || p.Box == position.Box));
}
