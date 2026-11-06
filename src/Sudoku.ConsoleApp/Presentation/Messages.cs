using Sudoku.Core.Domain;
using Sudoku.Core.Hints;
using Sudoku.Core.Validation;

namespace Sudoku.ConsoleApp.Presentation;

/// <summary>All user-facing text in one place, so wording stays consistent and is easy to change.</summary>
public static class Messages
{
    public const string Welcome = "Welcome to Sudoku!";
    public const string PuzzleHeader = "Here is your puzzle:";
    public const string CurrentGridHeader = "Current grid:";
    public const string Prompt = "Enter command (e.g., A3 4, C5 clear, hint, check, quit):";
    public const string MoveAccepted = "Move accepted.";
    public const string NoViolations = "No rule violations detected.";
    public const string NoHintAvailable = "No hint available: every cell is already correct.";
    public const string Completed = "You have successfully completed the Sudoku puzzle!";
    public const string PlayAgain = "Press any key to play again...";
    public const string Goodbye = "Thanks for playing. Goodbye!";
    public const string NumberOutOfRange = "Invalid move. Number must be between 1 and 9.";
    public const string EmptyInput = "Please enter a command.";

    public static string CellCleared(Position position) => $"Cell {position} cleared.";

    public static string CellIsPreFilled(Position position) => $"Invalid move. {position} is pre-filled.";

    public static string HintFor(Hint hint) => $"Hint: Cell {hint.Position} = {hint.Value}";

    public static string InvalidCell(string text) =>
        $"Invalid cell '{text}'. Use a row letter A-I followed by a column number 1-9, e.g. B3.";

    public static string UnknownCommand(string text) =>
        $"Unknown command '{text}'. Try A3 4, C5 clear, hint, check or quit.";

    public static string Violation(RuleViolation violation) => violation.Unit switch
    {
        UnitType.Row => $"Number {violation.Number} already exists in Row {Position.ToRowLabel(violation.UnitIndex)}.",
        UnitType.Column => $"Number {violation.Number} already exists in Column {violation.UnitIndex + 1}.",
        UnitType.Box => $"Number {violation.Number} already exists in the same 3×3 subgrid.",
        _ => throw new ArgumentOutOfRangeException(nameof(violation), violation.Unit, null),
    };
}
