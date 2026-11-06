using System.Text;
using Sudoku.ConsoleApp.Commands;
using Sudoku.ConsoleApp.Game;
using Sudoku.ConsoleApp.IO;
using Sudoku.Core.Generation;
using Sudoku.Core.Hints;
using Sudoku.Core.Solving;
using Sudoku.Core.Validation;

// Composition root: the only place concrete implementations are chosen.
// Usage: Sudoku.ConsoleApp [--seed <number>]   (a seed makes the puzzle sequence reproducible)

Console.OutputEncoding = Encoding.UTF8;

var random = TryGetSeed(args, out var seed) ? new Random(seed) : new Random();

var game = new SudokuGame(
    new SystemConsole(),
    new RandomPuzzleGenerator(new BacktrackingSolver(random), random),
    new GridValidator(),
    new RandomHintProvider(random),
    new CommandParser());

game.Run();

static bool TryGetSeed(string[] args, out int seed)
{
    seed = 0;
    var index = Array.FindIndex(args, arg => arg.Equals("--seed", StringComparison.OrdinalIgnoreCase));
    return index >= 0 && index + 1 < args.Length && int.TryParse(args[index + 1], out seed);
}
