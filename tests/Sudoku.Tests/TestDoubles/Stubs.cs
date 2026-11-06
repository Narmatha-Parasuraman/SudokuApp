using Sudoku.Core.Domain;
using Sudoku.Core.Generation;
using Sudoku.Core.Hints;

namespace Sudoku.Tests.TestDoubles;

public sealed class FixedPuzzleGenerator(Func<Puzzle> create) : IPuzzleGenerator
{
    public int GeneratedCount { get; private set; }

    public Puzzle Generate()
    {
        GeneratedCount++;
        return create();
    }
}

public sealed class FixedHintProvider(Hint? hint) : IHintProvider
{
    public Hint? GetHint(Board board) => hint;
}
