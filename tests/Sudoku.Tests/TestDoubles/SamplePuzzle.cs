using Sudoku.Core.Domain;

namespace Sudoku.Tests.TestDoubles;

/// <summary>The 30-clue puzzle used in the problem statement's examples, with its unique solution.</summary>
public static class SamplePuzzle
{
    public const string Givens = """
        53__7____
        6__195___
        _98____6_
        8___6___3
        4__8_3__1
        7___2___6
        _6____28_
        ___419__5
        ____8__79
        """;

    public const string Solution = """
        534678912
        672195348
        198342567
        859761423
        426853791
        713924856
        961537284
        287419635
        345286179
        """;

    public static Puzzle Create() => new(SudokuGrid.Parse(Givens), SudokuGrid.Parse(Solution));

    /// <summary>The moves that turn the givens into the solution, in row-major order.</summary>
    public static IEnumerable<(Position Position, int Value)> SolvingMoves()
    {
        var givens = SudokuGrid.Parse(Givens);
        var solution = SudokuGrid.Parse(Solution);
        return Position.All.Where(givens.IsEmpty).Select(p => (p, solution[p]));
    }
}
