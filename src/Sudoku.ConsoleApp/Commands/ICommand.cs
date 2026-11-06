using Sudoku.ConsoleApp.Game;

namespace Sudoku.ConsoleApp.Commands;

/// <summary>A single user action. New commands can be added without changing the game loop.</summary>
public interface ICommand
{
    CommandResult Execute(GameSession session);
}

/// <summary>What the game loop should show after a command runs.</summary>
/// <param name="Message">Text to display to the user.</param>
/// <param name="ShowGrid">Whether to redisplay the grid after the message.</param>
/// <param name="EndsGame">Whether the user asked to stop playing.</param>
public sealed record CommandResult(string Message, bool ShowGrid = false, bool EndsGame = false);
