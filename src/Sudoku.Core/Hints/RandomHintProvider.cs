using Sudoku.Core.Domain;

namespace Sudoku.Core.Hints;

/// <summary>
/// Reveals the solution value of a randomly chosen empty cell. Once no cells are empty it
/// reveals a cell the player has filled incorrectly, so hints remain useful until the puzzle is solved.
/// </summary>
public sealed class RandomHintProvider : IHintProvider
{
    private readonly Random _random;

    public RandomHintProvider(Random random)
    {
        _random = random ?? throw new ArgumentNullException(nameof(random));
    }

    public Hint? GetHint(Board board)
    {
        ArgumentNullException.ThrowIfNull(board);

        var incorrect = Position.All
            .Where(position => board.Grid[position] != board.Puzzle.Solution[position])
            .ToList();
        var empty = incorrect.Where(board.Grid.IsEmpty).ToList();
        var candidates = empty.Count > 0 ? empty : incorrect;

        if (candidates.Count == 0)
        {
            return null;
        }

        var position = candidates[_random.Next(candidates.Count)];
        return new Hint(position, board.Puzzle.Solution[position]);
    }
}
