using Sudoku.ConsoleApp.Game;
using Sudoku.ConsoleApp.Presentation;

namespace Sudoku.ConsoleApp.Commands;

public sealed class CheckCommand : ICommand
{
    public CommandResult Execute(GameSession session)
    {
        var violations = session.Validator.Validate(session.Board.Grid);
        var message = violations.Count == 0
            ? Messages.NoViolations
            : string.Join(Environment.NewLine, violations.Select(Messages.Violation));

        return new CommandResult(message);
    }
}
