using Sudoku.ConsoleApp.Game;

namespace Sudoku.ConsoleApp.Commands;

/// <summary>Stands in for input that could not be parsed; it only reports the problem.</summary>
public sealed record InvalidInputCommand(string Message) : ICommand
{
    public CommandResult Execute(GameSession session) => new(Message);
}
