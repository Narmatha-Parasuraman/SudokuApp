using Sudoku.ConsoleApp.Game;
using Sudoku.ConsoleApp.Presentation;

namespace Sudoku.ConsoleApp.Commands;

/// <summary>Reveals the correct value of one cell without changing the grid.</summary>
public sealed class HintCommand : ICommand
{
    public CommandResult Execute(GameSession session)
    {
        var hint = session.HintProvider.GetHint(session.Board);
        return new CommandResult(hint is null ? Messages.NoHintAvailable : Messages.HintFor(hint));
    }
}
