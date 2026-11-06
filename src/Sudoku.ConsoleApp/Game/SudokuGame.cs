using Sudoku.ConsoleApp.Commands;
using Sudoku.ConsoleApp.IO;
using Sudoku.ConsoleApp.Presentation;
using Sudoku.Core.Domain;
using Sudoku.Core.Generation;
using Sudoku.Core.Hints;
using Sudoku.Core.Validation;

namespace Sudoku.ConsoleApp.Game;

/// <summary>
/// The interactive game loop: generates a puzzle, reads commands until the puzzle is solved
/// or the user quits, and offers a new puzzle after each win.
/// </summary>
public sealed class SudokuGame
{
    private readonly IConsole _console;
    private readonly IPuzzleGenerator _generator;
    private readonly IGridValidator _validator;
    private readonly IHintProvider _hintProvider;
    private readonly CommandParser _parser;

    public SudokuGame(
        IConsole console,
        IPuzzleGenerator generator,
        IGridValidator validator,
        IHintProvider hintProvider,
        CommandParser parser)
    {
        _console = console ?? throw new ArgumentNullException(nameof(console));
        _generator = generator ?? throw new ArgumentNullException(nameof(generator));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _hintProvider = hintProvider ?? throw new ArgumentNullException(nameof(hintProvider));
        _parser = parser ?? throw new ArgumentNullException(nameof(parser));
    }

    private enum RoundOutcome
    {
        Solved,
        Quit,
    }

    public void Run()
    {
        _console.WriteLine(Messages.Welcome);

        while (PlayRound() == RoundOutcome.Solved)
        {
            _console.WriteLine();
            _console.WriteLine(Messages.Completed);
            _console.WriteLine(Messages.PlayAgain);
            if (!_console.WaitForKey())
            {
                return;
            }
        }
    }

    private RoundOutcome PlayRound()
    {
        var session = new GameSession(new Board(_generator.Generate()), _validator, _hintProvider);

        _console.WriteLine();
        _console.WriteLine(Messages.PuzzleHeader);
        ShowGrid(session.Board.Grid);

        while (true)
        {
            _console.WriteLine();
            _console.WriteLine(Messages.Prompt);
            var input = _console.ReadLine();
            if (input is null)
            {
                return RoundOutcome.Quit; // Input stream closed (e.g. Ctrl+Z / end of piped file).
            }

            var result = _parser.Parse(input).Execute(session);
            Display(result, session.Board.Grid);

            if (result.EndsGame)
            {
                return RoundOutcome.Quit;
            }

            if (session.IsSolved)
            {
                return RoundOutcome.Solved;
            }
        }
    }

    private void Display(CommandResult result, IReadOnlyGrid grid)
    {
        if (!result.ShowGrid)
        {
            _console.WriteLine(result.Message);
            return;
        }

        _console.WriteLine();
        _console.WriteLine(result.Message);
        _console.WriteLine();
        _console.WriteLine(Messages.CurrentGridHeader);
        ShowGrid(grid);
    }

    private void ShowGrid(IReadOnlyGrid grid) => _console.WriteLine(GridRenderer.Render(grid));
}
