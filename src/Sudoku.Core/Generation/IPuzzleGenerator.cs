using Sudoku.Core.Domain;

namespace Sudoku.Core.Generation;

public interface IPuzzleGenerator
{
    Puzzle Generate();
}
