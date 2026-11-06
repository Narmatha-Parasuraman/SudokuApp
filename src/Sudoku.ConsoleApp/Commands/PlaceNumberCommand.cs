using Sudoku.ConsoleApp.Game;
using Sudoku.ConsoleApp.Presentation;
using Sudoku.Core.Domain;

namespace Sudoku.ConsoleApp.Commands;

/// <summary>Places a number in a cell, e.g. "B3 7".</summary>
public sealed record PlaceNumberCommand(Position Position, int Number) : ICommand
{
    public CommandResult Execute(GameSession session)
    {
        var message = session.Board.Place(Position, Number) switch
        {
            MoveResult.Accepted => Messages.MoveAccepted,
            MoveResult.CellIsPreFilled => Messages.CellIsPreFilled(Position),
            MoveResult.NumberOutOfRange => Messages.NumberOutOfRange,
            var result => throw new InvalidOperationException($"Unhandled move result {result}."),
        };

        return new CommandResult(message, ShowGrid: true);
    }
}
