using Sudoku.ConsoleApp.Commands;
using Sudoku.ConsoleApp.Game;
using Sudoku.Core.Domain;
using Sudoku.Core.Hints;
using Sudoku.Core.Validation;
using Sudoku.Tests.TestDoubles;

namespace Sudoku.Tests.ConsoleApp;

public class CommandTests
{
    private readonly GameSession _session = new(
        new Board(SamplePuzzle.Create()),
        new GridValidator(),
        new FixedHintProvider(new Hint(new Position(4, 4), 5)));

    [Fact]
    public void PlaceNumber_Valid_AcceptsAndShowsGrid()
    {
        var result = new PlaceNumberCommand(new Position(0, 2), 4).Execute(_session);

        Assert.Equal(new CommandResult("Move accepted.", ShowGrid: true), result);
    }

    [Fact]
    public void PlaceNumber_PreFilled_ExplainsWhy()
    {
        var result = new PlaceNumberCommand(new Position(0, 0), 6).Execute(_session);

        Assert.Equal("Invalid move. A1 is pre-filled.", result.Message);
        Assert.True(result.ShowGrid);
    }

    [Fact]
    public void PlaceNumber_OutOfRange_ExplainsWhy()
    {
        var result = new PlaceNumberCommand(new Position(0, 2), 0).Execute(_session);

        Assert.Equal("Invalid move. Number must be between 1 and 9.", result.Message);
    }

    [Fact]
    public void ClearCell_UserEntry_ConfirmsAndShowsGrid()
    {
        _session.Board.Place(new Position(2, 4), 4);

        var result = new ClearCellCommand(new Position(2, 4)).Execute(_session);

        Assert.Equal(new CommandResult("Cell C5 cleared.", ShowGrid: true), result);
    }

    [Fact]
    public void ClearCell_PreFilled_ExplainsWhy()
    {
        var result = new ClearCellCommand(new Position(0, 0)).Execute(_session);

        Assert.Equal("Invalid move. A1 is pre-filled.", result.Message);
    }

    [Fact]
    public void Hint_FormatsCellAndValue()
    {
        Assert.Equal("Hint: Cell E5 = 5", new HintCommand().Execute(_session).Message);
    }

    [Fact]
    public void Hint_NoneAvailable_SaysSo()
    {
        var session = new GameSession(_session.Board, _session.Validator, new FixedHintProvider(null));

        Assert.Contains("No hint available", new HintCommand().Execute(session).Message);
    }

    [Fact]
    public void Check_NoConflicts_ReportsNoViolations()
    {
        Assert.Equal("No rule violations detected.", new CheckCommand().Execute(_session).Message);
    }

    [Theory]
    [InlineData(0, 2, 3, "Number 3 already exists in Row A.")]
    [InlineData(2, 0, 5, "Number 5 already exists in Column 1.")]
    [InlineData(1, 2, 8, "Number 8 already exists in the same 3×3 subgrid.")]
    public void Check_Conflicts_ReportsEachViolation(int row, int column, int number, string expectedLine)
    {
        _session.Board.Place(new Position(row, column), number);

        var lines = new CheckCommand().Execute(_session).Message.Split(Environment.NewLine);

        Assert.Contains(expectedLine, lines);
    }

    [Fact]
    public void Quit_EndsGame()
    {
        Assert.True(new QuitCommand().Execute(_session).EndsGame);
    }
}
