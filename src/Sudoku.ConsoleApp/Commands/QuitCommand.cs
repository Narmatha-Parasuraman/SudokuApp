using Sudoku.ConsoleApp.Game;
using Sudoku.ConsoleApp.Presentation;

namespace Sudoku.ConsoleApp.Commands;

public sealed class QuitCommand : ICommand
{
    public CommandResult Execute(GameSession session) => new(Messages.Goodbye, EndsGame: true);
}
