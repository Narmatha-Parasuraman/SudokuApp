using Sudoku.ConsoleApp.Game;
using Sudoku.ConsoleApp.Presentation;
using Sudoku.Core.Domain;

namespace Sudoku.ConsoleApp.Commands;

public sealed record ClearCellCommand(Position Position) : ICommand
{
    public CommandResult Execute(GameSession session)
    {
        var message = session.Board.Clear(Position) switch
        {
            MoveResult.Accepted => Messages.CellCleared(Position),
            MoveResult.CellIsPreFilled => Messages.CellIsPreFilled(Position),
            var result => throw new InvalidOperationException($"Unhandled move result {result}."),
        };

        return new CommandResult(message, ShowGrid: true);
    }
}
