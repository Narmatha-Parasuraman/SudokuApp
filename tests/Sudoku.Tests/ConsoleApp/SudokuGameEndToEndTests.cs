using Sudoku.ConsoleApp.Commands;
using Sudoku.ConsoleApp.Game;
using Sudoku.Core.Domain;
using Sudoku.Core.Generation;
using Sudoku.Core.Hints;
using Sudoku.Core.Solving;
using Sudoku.Core.Validation;
using Sudoku.Tests.TestDoubles;

namespace Sudoku.Tests.ConsoleApp;

/// <summary>Drives the whole game loop through scripted console input, mirroring the problem statement's examples.</summary>
public class SudokuGameEndToEndTests
{
    private const string Prompt = "Enter command (e.g., A3 4, C5 clear, hint, check, quit):";

    private const string StartingGrid = """
            1 2 3 4 5 6 7 8 9
          A 5 3 _ _ 7 _ _ _ _
          B 6 _ _ 1 9 5 _ _ _
          C _ 9 8 _ _ _ _ 6 _
          D 8 _ _ _ 6 _ _ _ 3
          E 4 _ _ 8 _ 3 _ _ 1
          F 7 _ _ _ 2 _ _ _ 6
          G _ 6 _ _ _ _ 2 8 _
          H _ _ _ 4 1 9 _ _ 5
          I _ _ _ _ 8 _ _ 7 9
        """;

    private readonly FixedPuzzleGenerator _generator = new(SamplePuzzle.Create);

    [Fact]
    public void Start_ShowsWelcomeAndPuzzle()
    {
        var console = Play("quit");

        Assert.StartsWith($"Welcome to Sudoku!\n\nHere is your puzzle:\n{Normalise(StartingGrid)}\n\n{Prompt}\n", console.Output);
    }

    [Fact]
    public void AcceptedMove_ShowsConfirmationAndUpdatedGrid()
    {
        var console = Play("A3 4", "quit");

        Assert.Contains("\nMove accepted.\n\nCurrent grid:\n", console.Output);
        Assert.Contains("  A 5 3 4 _ 7 _ _ _ _\n", console.Output);
    }

    [Fact]
    public void PreFilledCell_IsRejectedAndGridUnchanged()
    {
        var console = Play("A1 6", "quit");

        Assert.Contains($"\nInvalid move. A1 is pre-filled.\n\nCurrent grid:\n{Normalise(StartingGrid)}\n", console.Output);
    }

    [Theory]
    [InlineData("A3 3", "Number 3 already exists in Row A.")]
    [InlineData("C1 5", "Number 5 already exists in Column 1.")]
    [InlineData("B3 8", "Number 8 already exists in the same 3×3 subgrid.")]
    public void Check_AfterConflictingMove_ReportsViolation(string move, string expected)
    {
        var console = Play(move, "check", "quit");

        Assert.Contains("Move accepted.", console.Output);
        Assert.Contains($"\n{expected}\n", console.Output);
    }

    [Fact]
    public void Check_NoConflicts_ReportsClean()
    {
        var console = Play("A3 4", "check", "quit");

        Assert.Contains("No rule violations detected.", console.Output);
    }

    [Fact]
    public void Hint_RevealsACorrectValue()
    {
        var console = Play(new FixedHintProvider(new Hint(new Position(4, 4), 5)), "hint", "quit");

        Assert.Contains("Hint: Cell E5 = 5", console.Output);
    }

    [Fact]
    public void ClearCell_RemovesUserEntry()
    {
        var console = Play("A3 4", "A3 clear", "quit");

        Assert.Contains("Cell A3 cleared.", console.Output);
        Assert.EndsWith($"Current grid:\n{Normalise(StartingGrid)}\n\n{Prompt}\nThanks for playing. Goodbye!\n", console.Output);
    }

    [Fact]
    public void InvalidInput_IsReportedAndGameContinues()
    {
        var console = Play("Z1 4", "A3 x", "dance", "A3 4", "quit");

        Assert.Contains("Invalid cell 'Z1'", console.Output);
        Assert.Contains("Invalid move. Number must be between 1 and 9.", console.Output);
        Assert.Contains("Unknown command 'dance'", console.Output);
        Assert.Contains("Move accepted.", console.Output);
    }

    [Fact]
    public void Quit_EndsGameImmediately()
    {
        var console = Play("quit", "A3 4");

        Assert.EndsWith("Thanks for playing. Goodbye!\n", console.Output);
        Assert.DoesNotContain("Move accepted.", console.Output);
    }

    [Fact]
    public void EndOfInput_EndsGameWithoutError()
    {
        var console = Play("A3 4");

        Assert.EndsWith($"{Prompt}\n", console.Output);
    }

    [Fact]
    public void FillingTheGridCorrectly_WinsAndOffersANewGame()
    {
        var moves = SamplePuzzle.SolvingMoves().Select(m => $"{m.Position} {m.Value}").ToList();
        var console = Play([.. moves, "<any key>", "quit"]);

        Assert.Contains(
            "  I 3 4 5 2 8 6 1 7 9\n\nYou have successfully completed the Sudoku puzzle!\nPress any key to play again...\n",
            console.Output);
        Assert.Equal(1, console.KeyPresses);
        Assert.Equal(2, _generator.GeneratedCount);
    }

    [Fact]
    public void FullGridWithConflicts_DoesNotWin()
    {
        var moves = SamplePuzzle.SolvingMoves().Select(m => $"{m.Position} {m.Value}").ToList();
        moves[0] = "A3 3"; // Breaks row A instead of placing the correct 4.
        var console = Play([.. moves, "quit"]);

        Assert.DoesNotContain("successfully completed", console.Output);
    }

    [Fact]
    public void RealGenerator_StartsWithThirtyPrefilledCells()
    {
        var random = new Random(123);
        var generator = new RandomPuzzleGenerator(new BacktrackingSolver(random), random);
        var console = new FakeConsole("quit");

        new SudokuGame(console, generator, new GridValidator(), new RandomHintProvider(random), new CommandParser()).Run();

        var gridRows = console.Output.Split('\n').Where(line => line.Length > 2 && line[2] is >= 'A' and <= 'I').Take(9);
        var filled = gridRows.Sum(row => row[4..].Count(char.IsDigit));
        Assert.Equal(30, filled);
    }

    private FakeConsole Play(params string[] input) => Play(new FixedHintProvider(null), input);

    private FakeConsole Play(IHintProvider hintProvider, params string[] input)
    {
        var console = new FakeConsole(input);
        new SudokuGame(console, _generator, new GridValidator(), hintProvider, new CommandParser()).Run();
        return console;
    }

    private static string Normalise(string text) => text.ReplaceLineEndings("\n");
}
